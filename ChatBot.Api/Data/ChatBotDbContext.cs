using ChatBot.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ChatBot.Api.Data
{
    public class ChatBotDbContext : DbContext
    {
        public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options) : base(options)
        {
        }

        public DbSet<TextEmbedding> Embeddings { get; set; }
    }
}
