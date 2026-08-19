using System.Collections.Generic;
using System.Xml.Serialization;

namespace DiscordAPI.API
{
    public interface IDiscordClient
    {
        bool IsEnabled { get; }
        void SendPayload(string json);
    }

    [XmlType("DiscordEmbed")]
    public class DiscordEmbed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Color { get; set; }
        public DiscordFooter Footer { get; set; }
        public DiscordThumbnail Thumbnail { get; set; }
        public DiscordImage Image { get; set; }
        public DiscordAuthor Author { get; set; }
        public List<DiscordField> Fields { get; set; } = new List<DiscordField>();

        public void AddField(string name, string value, bool inline = false)
        {
            Fields.Add(new DiscordField { Name = name, Value = value, Inline = inline });
        }
    }

    public class DiscordField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Inline { get; set; }
    }

    public class DiscordFooter
    {
        public string Text { get; set; }
        public string IconUrl { get; set; }
    }

    public class DiscordThumbnail
    {
        public string Url { get; set; }
    }

    public class DiscordImage
    {
        public string Url { get; set; }
    }

    public class DiscordAuthor
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string IconUrl { get; set; }
    }
}
