namespace DevBlog.API.DTOs
{
    public record PostDto(
    int Id,
    string Title,
    string Slug,
    string Summary,
    string Content,
    string Thumbnail,
    DateTime PublishedAt
);
}
