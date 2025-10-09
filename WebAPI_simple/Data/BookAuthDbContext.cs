using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebAPI_simple.Data
{
    // Cần đảm bảo tên class là BookAuthDbContext để khớp với Program.cs
    public class BookAuthDbContext : IdentityDbContext<IdentityUser>
    {
        public BookAuthDbContext(DbContextOptions<BookAuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // BẮT BUỘC: Gọi Base method để Identity tạo các bảng AspNet*
            base.OnModelCreating(builder);

            // Tạo phân quyền Reader và Writer
            var readerRoleId = "004c7e807dfc44be89522c7130898655";
            var writeRoleId = "71e282d3-76ca-485e-b094-eff019287fa5";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name = "Read",
                    NormalizedName = "READ" // .ToUpper()
                },
                new IdentityRole
                {
                    Id = writeRoleId,
                    ConcurrencyStamp = writeRoleId,
                    Name = "Write",
                    NormalizedName = "WRITE" // .ToUpper()
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}