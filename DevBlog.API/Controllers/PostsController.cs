using DevBlog.API.Data;
using DevBlog.API.DTOs;
using DevBlog.API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PostsController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/posts
        [HttpGet]
        public async Task<IEnumerable<PostDto>> GetAll()
        {
            return await _db.Posts
                .OrderByDescending(p => p.PublishedAt)
                .Select(p => new PostDto(
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.Summary,
                    p.Content,
                    p.Thumbnail,
                    p.PublishedAt
                ))
                .ToListAsync();
        }

        // GET /api/posts/{slug}
        [HttpGet("{slug}")]
        public async Task<ActionResult<PostDto>> GetBySlug(string slug)
        {
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Slug == slug);

            if (post == null)
                return NotFound();

            return new PostDto(
                post.Id,
                post.Title,
                post.Slug,
                post.Summary,
                post.Content,
                post.Thumbnail,
                post.PublishedAt
            );
        }
        // GET /api/posts/id/{id}
        [HttpGet("id/{id}")]
        public async Task<ActionResult<PostDto>> GetById(int id)
        {
            var post = await _db.Posts.FindAsync(id);

            if (post == null)
                return NotFound();

            return new PostDto(
                post.Id, post.Title, post.Slug, post.Summary,
                post.Content, post.Thumbnail, post.PublishedAt
            );
        }

        // POST /api/posts
        [HttpPost]
        public async Task<ActionResult<PostDto>> Create(PostDto createDto)
        {
            // Chuyển đổi từ DTO sang Entity (Giả định class Entity của bạn là Post)
            var post = new Post
            {
                Title = createDto.Title,
                Slug = createDto.Slug,
                Summary = createDto.Summary,
                Content = createDto.Content,
                Thumbnail = createDto.Thumbnail,
                PublishedAt = DateTime.UtcNow // Gán thời gian hiện tại
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            // Trả về kết quả kèm theo đường dẫn truy cập (201 Created)
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, new PostDto(
                post.Id, post.Title, post.Slug, post.Summary,
                post.Content, post.Thumbnail, post.PublishedAt
            ));
        }

        // PUT /api/posts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PostDto updateDto)
        {
            var post = await _db.Posts.FindAsync(id);

            if (post == null)
                return NotFound();

            // Cập nhật các trường
            post.Title = updateDto.Title;
            post.Slug = updateDto.Slug;
            post.Summary = updateDto.Summary;
            post.Content = updateDto.Content;
            post.Thumbnail = updateDto.Thumbnail;
            // post.PublishedAt thường không cập nhật khi sửa bài

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id)) return NotFound();
                else throw;
            }

            return NoContent(); // 204 No Content
        }

        // DELETE /api/posts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null)
                return NotFound();

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _db.Posts.Any(e => e.Id == id);
        }
    }
}
