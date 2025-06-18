using ChatBot.Api.Models;
using ChatBot.Api.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChatBot.Api.Data
{
    public class ChatBotDbContext : IdentityDbContext<User, Role, Guid>
    {
        public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : base(options)
        {
        }

        public DbSet<TextEmbedding> Embeddings { get; set; }
    }
}
