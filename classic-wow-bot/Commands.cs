using AngleSharp;
using AngleSharp.Html.Parser;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace ClassicWoWBot
{
    public class ServerCommands
    {
        private static readonly HttpClient client = new HttpClient();

        [Command("status")] // let's define this method as a command
        [Description("Shows the current status of the Light's Hope classic server.")] // this will be displayed to tell users what this command does when they invoke help
        public async Task Status(CommandContext context) // this command takes no arguments
        {
            // let's trigger a typing indicator to let users know we're working
            await context.TriggerTypingAsync();

            // Use the default configuration for AngleSharp
            var config = Configuration.Default;

            // Create a new context for evaluating webpages with the given config
            var browseContext = BrowsingContext.New(config);

            // Source to be parsed
            string response = await client.GetStringAsync("https://status.lightshope.org/");

            // Create a virtual request to specify the document to load (here from our fixed string)
            var document = await browseContext.OpenAsync(req => req.Content(response));

            // Grab the first relevant section, <div class="alert ..."></div> from the doc
            var generalStatusDiv = document.QuerySelector("div.alert");
            string generalStatus = generalStatusDiv.TextContent;

            // let's make the message a bit more colourful
            DiscordEmoji generalStatusEmoji;
            if (generalStatus.Contains("operational"))
            {
                // Set to 👌
                generalStatusEmoji = DiscordEmoji.FromName(context.Client, ":ok_hand:");
            }
            else
            {
                // Set to 👎
                generalStatusEmoji = DiscordEmoji.FromName(context.Client, ":thumbsdown:");
            }

            // Grab all <small class="text-component-1 ..."></small> from the doc
            var allSmall = document.QuerySelectorAll("small.text-component-1");

            // the sixth element in this list is the Northdale server
            string serverStatus = $"Northdale Server is {allSmall[5].TextContent.ToLower()}";

            DiscordEmoji serverStatusEmoji;
            DiscordColor textColor;
            if (generalStatus.Contains("operational"))
            {
                // Set to 👌
                serverStatusEmoji = DiscordEmoji.FromName(context.Client, ":ok_hand:");

                textColor = DiscordColor.PhthaloGreen;
            }
            else
            {
                // Set to 👎
                serverStatusEmoji = DiscordEmoji.FromName(context.Client, ":thumbsdown:");

                textColor = DiscordColor.Red;
            }

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "Light's Hope Status",
                Description = $"{serverStatusEmoji} {serverStatus} | {generalStatusEmoji} {generalStatus}",

                Timestamp = DateTime.Now
            };

            // text color green if operational, otherwise red
            embed.WithColor(textColor);

            // respond with content
            await context.RespondAsync(embed: embed);
        }

        [Command("incidents")]
        [Description("Says whether any incidents have occurred today in Light's Hope servers.")]
        public async Task Incidents(CommandContext context)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await context.TriggerTypingAsync();

            // Use the default configuration for AngleSharp
            var config = Configuration.Default;

            // Create a new context for evaluating webpages with the given config
            var browseContext = BrowsingContext.New(config);

            // Source to be parsed
            string response = await client.GetStringAsync("https://status.lightshope.org/");

            // Create a virtual request to specify the document to load (here from our fixed string)
            var document = await browseContext.OpenAsync(req => req.Content(response));

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "Today's Incidents on Light's Hope"
            };

            // Grab the first relevant section, <div class="alert ..."></div> from the doc
            var incidentDiv = document.QuerySelector("div.panel-body > p");
            string incidentList = incidentDiv.TextContent;

            DiscordEmoji incidentStatusEmoji;
            DiscordColor textColor;
            string incidentStatus;
            if (incidentList.Equals("No incidents reported"))
            {
                // Set to 👌
                incidentStatusEmoji = DiscordEmoji.FromName(context.Client, ":ok_hand:");
                incidentStatus = "No incidents reported today.";
            }
            else
            {
                // Set to ❗
                incidentStatusEmoji = DiscordEmoji.FromName(context.Client, ":exclamation:");
                incidentStatus = "There was an incident today.";
                textColor = DiscordColor.Gold;
                embed.WithColor(textColor);
                embed.WithUrl("https://status.lightshope.org/");
            }

            embed.WithDescription($"{incidentStatusEmoji} {incidentStatus}");

            // send response
            await context.RespondAsync(embed: embed);
        }
    }

    public class WowHeadCommands
    {
        private static readonly HttpClient client = new HttpClient();

        [Command("linkerator")] // let's define this method as a command
        [Description("Shows the current status of the Light's Hope classic server.")] // this will be displayed to tell users what this command does when they invoke help
        public async Task Linkerator(CommandContext context) // this command takes no arguments
        {
            // let's trigger a typing indicator to let users know we're working
            await context.TriggerTypingAsync();

            // Use the default configuration for AngleSharp
            var config = Configuration.Default;

            // Create a new context for evaluating webpages with the given config
            var browseContext = BrowsingContext.New(config);

            // Source to be parsed
            string response = await client.GetStringAsync("https://status.lightshope.org/");

            // Create a virtual request to specify the document to load (here from our fixed string)
            var document = await browseContext.OpenAsync(req => req.Content(response));

            // Grab the first relevant section, <div class="alert ..."></div> from the doc
            var generalStatusDiv = document.QuerySelector("div.alert");
            string generalStatus = generalStatusDiv.TextContent;

            // let's make the message a bit more colourful
            DiscordEmoji generalStatusEmoji;
            if (generalStatus.Contains("operational"))
            {
                // Set to 👌
                generalStatusEmoji = DiscordEmoji.FromName(context.Client, ":ok_hand:");
            }
            else
            {
                // Set to 👎
                generalStatusEmoji = DiscordEmoji.FromName(context.Client, ":thumbsdown:");
            }

            // Grab all <small class="text-component-1 ..."></small> from the doc
            var allSmall = document.QuerySelectorAll("small.text-component-1");

            // the sixth element in this list is the Northdale server
            string serverStatus = $"Northdale Server is {allSmall[5].TextContent.ToLower()}";

            DiscordEmoji serverStatusEmoji;
            DiscordColor textColor;
            if (generalStatus.Contains("operational"))
            {
                // Set to 👌
                serverStatusEmoji = DiscordEmoji.FromName(context.Client, ":ok_hand:");

                textColor = DiscordColor.PhthaloGreen;
            }
            else
            {
                // Set to 👎
                serverStatusEmoji = DiscordEmoji.FromName(context.Client, ":thumbsdown:");

                textColor = DiscordColor.Red;
            }

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "Light's Hope Status",
                Description = $"{serverStatusEmoji} {serverStatus} | {generalStatusEmoji} {generalStatus}",

                Timestamp = DateTime.Now
            };

            // text color green if operational, otherwise red
            embed.WithColor(textColor);

            // respond with content
            await context.RespondAsync(embed: embed);
        }

        [Command("find")]
        [Description("Says whether any incidents have occurred today in Light's Hope servers.")]
        public async Task Find(CommandContext context)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await context.TriggerTypingAsync();

            // Use the default configuration for AngleSharp
            var config = Configuration.Default;

            // Create a new context for evaluating webpages with the given config
            var browseContext = BrowsingContext.New(config);

            // Source to be parsed
            string response = await client.GetStringAsync("https://status.lightshope.org/");

            // Create a virtual request to specify the document to load (here from our fixed string)
            var document = await browseContext.OpenAsync(req => req.Content(response));

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "Today's Incidents on Light's Hope"
            };

            // Grab the first relevant section, <div class="alert ..."></div> from the doc
            var incidentDiv = document.QuerySelector("div.panel-body > p");
            string incidentList = incidentDiv.TextContent;

            DiscordEmoji incidentStatusEmoji;
            DiscordColor textColor;
            string incidentStatus;
            if (incidentList.Equals("No incidents reported"))
            {
                // Set to 👌
                incidentStatusEmoji = DiscordEmoji.FromName(context.Client, ":ok_hand:");
                incidentStatus = "No incidents reported today.";
            }
            else
            {
                // Set to ❗
                incidentStatusEmoji = DiscordEmoji.FromName(context.Client, ":exclamation:");
                incidentStatus = "There was an incident today.";
                textColor = DiscordColor.Gold;
                embed.WithColor(textColor);
                embed.WithUrl("https://status.lightshope.org/");
            }

            embed.WithDescription($"{incidentStatusEmoji} {incidentStatus}");

            // send response
            await context.RespondAsync(embed: embed);
        }
    }
}