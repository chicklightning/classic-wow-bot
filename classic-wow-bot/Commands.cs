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
using AngleSharp.Html.Dom;

namespace ClassicWoWBot
{
    public class GeneralCommands
    {
        private static readonly HttpClient client = new HttpClient();

        [Command("about")] // let's define this method as a command
        [Description("Gives more information about the bot.")] // this will be displayed to tell users what this command does when they invoke help
        public async Task About(CommandContext context) // this command takes no arguments
        {
            // let's trigger a typing indicator to let users know we're working
            await context.TriggerTypingAsync();

            DiscordEmoji codeEmoji = DiscordEmoji.FromName(context.Client, ":space_invader:");

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "About Classic WoW Bot",
                Description = $"{codeEmoji} This is a link to my GitHub repository! From there you can view a more in-depth explanation of bot commands, and get a link to my DiscordBots page.",
                Url = "https://github.com/chicklightning/classic-wow-bot",
                Color = DiscordColor.Purple
            };

            // respond with content
            await context.RespondAsync(embed: embed);
        }
    }

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

            // Grab the first relevant section, first <p></p> in <div class="panel-body ..."></div> from the doc
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
        [Description("Type the name of an existing item and it will show up with a link in chat.")] // this will be displayed to tell users what this command does when they invoke help
        public async Task Linkerator(CommandContext context, [Description("The item you want to link.")] string item)
        {
            // let's trigger a typing indicator to let users know we're working
            await context.TriggerTypingAsync();

            // Use the default configuration for AngleSharp
            var config = Configuration.Default;

            // Create a new context for evaluating webpages with the given config
            var browseContext = BrowsingContext.New(config);

            string apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
            string customSearchEngine = Environment.GetEnvironmentVariable("SEARCH_ENGINE_KEY");
            var svc = new Google.Apis.Customsearch.v1.CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer { ApiKey = apiKey });
            var listRequest = svc.Cse.List(item);

            listRequest.Cx = customSearchEngine;
            listRequest.Num = 5; // get first 5 search results
            var searchItems = listRequest.Execute();

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder();
            embed.WithFooter($"Search results for {item}...");

            bool itemFound = false;
            var searchItem = new Google.Apis.Customsearch.v1.Data.Result();
            if (searchItems.Items != null)
            {
                foreach (var currentItem in searchItems.Items)
                {
                    if (currentItem.Title.Contains("Item") && !itemFound)
                    {
                        itemFound = true;
                        searchItem = currentItem;
                    }
                }
            }

            if (itemFound) // got a result, and it's an item!
            {
                embed.WithTitle(searchItem.Title.Replace(" - Item - World of Warcraft", ""));
                embed.WithDescription(searchItem.Snippet);
                embed.WithUrl(searchItem.Link);

                // Source to be parsed
                string response = await client.GetStringAsync(searchItem.Link);

                var document = await browseContext.OpenAsync(req => req.Content(response));

                // all <link> tags
                var links = document.QuerySelectorAll("link");
                foreach (var link in links)
                {
                    // looking for <link rel="image_src" href="item icon image">
                    if (link.HasAttribute("rel") && link.GetAttribute("rel").Contains("image_src") &&
                        link.HasAttribute("href"))
                    {
                        embed.WithThumbnailUrl(link.GetAttribute("href"));
                    }
                }
            }
            else // not an item
            {
                DiscordEmoji searchEmoji = DiscordEmoji.FromName(context.Client, ":grimacing:");
                embed.WithTitle($"{searchEmoji} Couldn't find a page for this item, is it possible you misspelled it or it isn't an item from Classic WoW?");
            }

            // respond with content
            await context.RespondAsync(embed: embed);
        }

        [Command("find")]
        [Description("Returns the first result from Classic WoWHead for your search term.")]
        public async Task Find(CommandContext context, [Description("The term you want to search for.")] string term)
        {
            // let's trigger a typing indicator to let users know we're working
            await context.TriggerTypingAsync();

            // Use the default configuration for AngleSharp
            var config = Configuration.Default;

            // Create a new context for evaluating webpages with the given config
            var browseContext = BrowsingContext.New(config);

            string apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
            string customSearchEngine = Environment.GetEnvironmentVariable("SEARCH_ENGINE_KEY");
            var svc = new Google.Apis.Customsearch.v1.CustomsearchService(new Google.Apis.Services.BaseClientService.Initializer { ApiKey = apiKey });
            var listRequest = svc.Cse.List(term);

            listRequest.Cx = customSearchEngine;
            var searchItem = listRequest.Execute().Items; // get the first search result

            // wrap it into an embed
            var embed = new DiscordEmbedBuilder();
            embed.WithFooter($"Search results for {term}...");

            if (searchItem != null) // found a search result!
            {
                embed.WithTitle(searchItem[0].Title.Replace(" - World of Warcraft", ""));
                embed.WithDescription(searchItem[0].Snippet);
                embed.WithUrl(searchItem[0].Link);
            }
            else // not an item
            {
                DiscordEmoji searchEmoji = DiscordEmoji.FromName(context.Client, ":grimacing:");
                embed.WithTitle($"{searchEmoji} Couldn't find a page for this term, is it possible you misspelled it or it isn't from Classic WoW?");
            }

            // respond with content
            await context.RespondAsync(embed: embed);
        }
    }
}