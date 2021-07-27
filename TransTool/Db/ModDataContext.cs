using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace TransTool.Db
{
    public class ModDataContext : DbContext
    {
        public DbSet<ModInfo> ModInfos { get; set; }
        public DbSet<TranslationHistory> TranslationHistories { get; set; }
        public DbSet<LanguageKeyValuePair> LanguageKeyValuePairs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./data/mod_translation.db");
            //optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }
    }

    [Table("mod_info")]
    public class ModInfo
    {
        [Key]
        public Guid Guid { get; set; }
        public string ModName { get; set; }
        public long CurseForgeId { get; set; }
        public string ModSlug { get; set; }
        public string ModNickName { get; set; }
        public List<TranslationHistory> TranslationHistories { get; set; }
    }

    [Table("translation_history")]
    public class TranslationHistory
    {
        [Key]
        public Guid Guid { get; set; }
        public DateTimeOffset TimeOffset { get; set; }
        public string Memo { get; set; }
        [ForeignKey("Guid")]
        public List<LanguageKeyValuePair> LanguageKeyValuePairs { get; set; }
    }

    [Table("language_kv")]
    public class LanguageKeyValuePair
    {
        [Key]
        public Guid Guid { get; set; }
        public string Key { get; set; }
        public string Origin { get; set; }
        public string Localized { get; set; }
    }
}