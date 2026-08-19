using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordAPI.API
{
    public static class DiscordPayloads
    {
        public static string Serialize(string content, DiscordEmbed embed = null, string channelType = "Default")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            
            // Internal relay routing info
            sb.Append("\"channel_type\": \"").Append(channelType).Append("\",");

            if (!string.IsNullOrEmpty(content))
            {
                sb.Append("\"content\": \"").Append(EscapeJson(content)).Append("\"");
                if (embed != null) sb.Append(",");
            }

            if (embed != null)
            {
                sb.Append("\"embeds\": [");
                SerializeEmbed(sb, embed);
                sb.Append("]");
            }

            sb.Append("}");
            return sb.ToString();
        }

        private static void SerializeEmbed(StringBuilder sb, DiscordEmbed embed)
        {
            sb.Append("{");
            bool first = true;

            if (!string.IsNullOrEmpty(embed.Title))
            {
                AppendProperty(sb, "title", embed.Title, ref first);
            }

            if (!string.IsNullOrEmpty(embed.Description))
            {
                AppendProperty(sb, "description", embed.Description, ref first);
            }

            if (!string.IsNullOrEmpty(embed.Url))
            {
                AppendProperty(sb, "url", embed.Url, ref first);
            }

            if (embed.Color != 0)
            {
                if (!first) sb.Append(",");
                sb.Append("\"color\": ").Append(embed.Color);
                first = false;
            }

            if (embed.Fields != null && embed.Fields.Count > 0)
            {
                if (!first) sb.Append(",");
                sb.Append("\"fields\": [");
                for (int i = 0; i < embed.Fields.Count; i++)
                {
                    var field = embed.Fields[i];
                    sb.Append("{");
                    sb.Append("\"name\": \"").Append(EscapeJson(field.Name)).Append("\",");
                    sb.Append("\"value\": \"").Append(EscapeJson(field.Value)).Append("\",");
                    sb.Append("\"inline\": ").Append(field.Inline.ToString().ToLower());
                    sb.Append("}");
                    if (i < embed.Fields.Count - 1) sb.Append(",");
                }
                sb.Append("]");
                first = false;
            }

            if (embed.Author != null)
            {
                if (!first) sb.Append(",");
                sb.Append("\"author\": {");
                bool authorFirst = true;
                AppendProperty(sb, "name", embed.Author.Name, ref authorFirst);
                AppendProperty(sb, "url", embed.Author.Url, ref authorFirst);
                AppendProperty(sb, "icon_url", embed.Author.IconUrl, ref authorFirst);
                sb.Append("}");
                first = false;
            }

            if (embed.Footer != null)
            {
                if (!first) sb.Append(",");
                sb.Append("\"footer\": {");
                bool footerFirst = true;
                AppendProperty(sb, "text", embed.Footer.Text, ref footerFirst);
                AppendProperty(sb, "icon_url", embed.Footer.IconUrl, ref footerFirst);
                sb.Append("}");
                first = false;
            }

            if (embed.Thumbnail != null)
            {
                if (!first) sb.Append(",");
                sb.Append("\"thumbnail\": {\"url\": \"").Append(EscapeJson(embed.Thumbnail.Url)).Append("\"}");
                first = false;
            }

            if (embed.Image != null)
            {
                if (!first) sb.Append(",");
                sb.Append("\"image\": {\"url\": \"").Append(EscapeJson(embed.Image.Url)).Append("\"}");
                first = false;
            }

            sb.Append("}");
        }

        private static void AppendProperty(StringBuilder sb, string name, string value, ref bool first)
        {
            if (string.IsNullOrEmpty(value)) return;
            if (!first) sb.Append(",");
            sb.Append("\"").Append(name).Append("\": \"").Append(EscapeJson(value)).Append("\"");
            first = false;
        }

        private static string EscapeJson(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
        }
    }
}
