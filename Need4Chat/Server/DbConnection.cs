using Microsoft.EntityFrameworkCore;

namespace Need4Chat.Server.Models
{

	public partial class database1Context : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			string ConnectionString = Startup.Configuration["ConnectionString:Burbank"];
			optionsBuilder.UseSqlServer(ConnectionString);

			base.OnConfiguring(optionsBuilder);
		}
	}
}
