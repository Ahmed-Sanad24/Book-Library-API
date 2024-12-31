using Microsoft.AspNetCore.Mvc;
using Bill_system_API.Models;
using AutoMapper;
using Bill_system_API.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Bill_system_API.DTOs;

namespace Bill_system_API.Controllers
{
    /// <summary>
    /// Handles book-related actions.
    /// </summary>
    [Route("api/books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public BookController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets a list of all books.
        /// </summary>
        /// <returns>A list of books.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public ActionResult<IEnumerable<Book>> GetBooks()
        {
            var books = unitOfWork.Books.GetAll().ToList();
            return Ok(books);
        }

        /// <summary>
        /// Gets the details of a specific book.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book details.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public ActionResult<Book> GetBook(int id)
        {
            var book = unitOfWork.Books.getById(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        /// <summary>
        /// Updates the details of a specific book.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="updateBookDTO">The updated book details.</param>
        /// <returns>A status message indicating the result of the update process.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult PutBook(int id, UpdateBookDTO updateBookDTO)
        {
            if (id != updateBookDTO.Id)
            {
                return BadRequest();
            }
            Book bookDB = unitOfWork.Books.getById(id);
            if (bookDB == null)
            {
                return NotFound();
            }
            bookDB.Title = updateBookDTO.Title;
            bookDB.Author = updateBookDTO.Author;
            bookDB.PublishedYear = updateBookDTO.PublishedYear;
            bookDB.IsAvailable = updateBookDTO.IsAvailable;
            unitOfWork.Books.update(bookDB);
            unitOfWork.Complete();

            return NoContent();
        }

        /// <summary>
        /// Adds a new book to the library.
        /// </summary>
        /// <param name="createBookDTO">The details of the book to create.</param>
        /// <returns>The created book.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<CreateBookDTO> PostBook(CreateBookDTO createBookDTO)
        {
            if (createBookDTO == null)
            {
                return BadRequest();
            }
            Book bookNew = mapper.Map<Book>(createBookDTO);
            unitOfWork.Books.add(bookNew);
            unitOfWork.Complete();
            return CreatedAtAction("GetBook", new { id = bookNew.Id }, bookNew);
        }

        /// <summary>
        /// Deletes a specific book from the library.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <returns>A status message indicating the result of the delete process.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var bookDB = unitOfWork.Books.getById(id);
            if (bookDB == null)
            {
                return NotFound();
            }

            unitOfWork.Books.delete(bookDB);
            unitOfWork.Complete();
            return NoContent();
        }
    }
}
