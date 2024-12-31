using Microsoft.AspNetCore.Mvc;
using Bill_system_API.Models;
using AutoMapper;
using Bill_system_API.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Bill_system_API.DTOs;
using System.Security.Claims;

namespace Bill_system_API.Controllers
{
    /// <summary>
    /// Handles actions related to borrowed books.
    /// </summary>
    [Route("api/borrow")]
    [ApiController]
    public class BorrowedBookController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public BorrowedBookController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Borrows a book for a user.
        /// </summary>
        /// <param name="bookId">The ID of the book to borrow.</param>
        /// <returns>A status message indicating the result of the borrow process.</returns>
        [HttpPost("{bookId}")]
        [Authorize(Roles = "User")]
        public IActionResult BorrowBook(int bookId)
        {
            var book = unitOfWork.Books.getById(bookId);
            if (book == null || !book.IsAvailable)
            {
                return BadRequest("Book is not available for borrowing.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowedBook = new BorrowedBook
            {
                BookId = bookId,
                UserId = userId,
                BorrowedDate = DateTime.Now,
                ReturnDate = null
            };

            book.IsAvailable = false;
            unitOfWork.BorrowedBooks.add(borrowedBook);
            unitOfWork.Books.update(book);
            unitOfWork.Complete();

            return Ok("Book borrowed successfully.");
        }

        /// <summary>
        /// Gets the list of borrowed books for the current user.
        /// </summary>
        /// <returns>A list of borrowed books.</returns>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult<IEnumerable<BorrowedBookDTO>> GetBorrowedBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowedBooks = unitOfWork.BorrowedBooks.Find(b => b.UserId == userId).ToList();
            var borrowedBooksDTO = mapper.Map<IEnumerable<BorrowedBookDTO>>(borrowedBooks);

            return Ok(borrowedBooksDTO);
        }

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        /// <param name="bookId">The ID of the book to return.</param>
        /// <returns>A status message indicating the result of the return process.</returns>
        [HttpPost("return/{bookId}")]
        [Authorize(Roles = "User")]
        public IActionResult ReturnBook(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowedBook = unitOfWork.BorrowedBooks.Find(b => b.BookId == bookId && b.UserId == userId && b.ReturnDate == null).FirstOrDefault();

            if (borrowedBook == null)
            {
                return BadRequest("No borrowed book found for the given book ID.");
            }
            // we can add this logic
            //if (borrowedBook.IsOverdue())
            //{
            //    return BadRequest("Book is overdue. Please pay the fine.");
            //}

            borrowedBook.ReturnDate = DateTime.Now;
            var book = unitOfWork.Books.getById(bookId);
            book.IsAvailable = true;

            unitOfWork.BorrowedBooks.update(borrowedBook);
            unitOfWork.Books.update(book);
            unitOfWork.Complete();

            return Ok("Book returned successfully.");
        }
    }
}
