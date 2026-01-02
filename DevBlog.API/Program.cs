using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using DevBlog.API.Data; // Thay bằng namespace thực tế của bạn

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext với PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()//fix client url thực tế sau
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
// Thêm đoạn này để tự động tạo bảng khi API khởi động lần đầu trên Server
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
// 3. Cấu hình Forwarded Headers (BẮT BUỘC khi chạy sau Nginx trên Linux)
// Giúp .NET hiểu được giao thức https và IP thật từ Nginx gửi tới
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Lưu ý: Khi chạy trên Linux với Nginx, thường Nginx đã lo phần HTTPS (SSL)
// nên đôi khi bạn có thể comment dòng UseHttpsRedirection nếu bị lỗi vòng lặp redirect
app.UseHttpsRedirection();

// 4. Kích hoạt CORS (Phải nằm trước MapControllers)
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();