using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using MyList.Models;
using System.Xml.Linq;
using Microsoft.Toolkit.Uwp.Notifications;
using MyList.ViewModels;

namespace MyList.Services
{
    class TileService
    {
        static public void SetBadgeCountOnTile(int count)
        {
            // Update the badge on the real tile
            XmlDocument badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            XmlElement badgeElement = (XmlElement)badgeXml.SelectSingleNode("/badge");
            badgeElement.SetAttribute("value", count.ToString());

            BadgeNotification badge = new BadgeNotification(badgeXml);
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge);
        }

        /*public static XmlDocument CreateTiles(PrimaryTile primaryTile)
        {
            XDocument xDoc = new XDocument(
                new XElement("tile", new XAttribute("version", 3),
                    new XElement("visual",
                        // Small Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileSmall"),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile.time, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile.message, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),

                        // Medium Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileMedium"),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile.time, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile.message, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                )
                            )
                        ),

                        // Wide Tile
                        new XElement("binding", new XAttribute("branding", primaryTile.branding), new XAttribute("displayName", primaryTile.appName), new XAttribute("template", "TileWide"),
                            new XElement("group",
                                new XElement("subgroup",
                                    new XElement("text", primaryTile.time, new XAttribute("hint-style", "caption")),
                                    new XElement("text", primaryTile.message, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3)),
                                    new XElement("text", primaryTile.message2, new XAttribute("hint-style", "captionsubtle"), new XAttribute("hint-wrap", true), new XAttribute("hint-maxLines", 3))
                                ),
                                new XElement("subgroup", new XAttribute("hint-weight", 15),
                                    new XElement("image", new XAttribute("placement", "inline"), new XAttribute("src", "Assets/StoreLogo.png"))
                                )
                            )
                        )
                    )
                )
            );

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xDoc.ToString());
            return xmlDoc;
        }*/

        public static void UpdateTiles()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            ListItemViewModel viewModel = ListItemViewModel.GetViewModel();
            TileContent content;

            for (int i = 0; i < viewModel.AllItems.Count && i < 5; ++i)
            {
                content = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        Branding = TileBranding.NameAndLogo,
                        TileSmall = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = viewModel.AllItems[i].Title, HintWrap = true},
                                    new AdaptiveText() { Text = viewModel.AllItems[i].description, HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        },

                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = viewModel.AllItems[i].Title, HintWrap = true },
                                    new AdaptiveText() { Text = viewModel.AllItems[i].description, HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = viewModel.AllItems[i].Title, HintWrap = true },
                                    new AdaptiveText() { Text = viewModel.AllItems[i].description, HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        },

                        TileLarge = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = viewModel.AllItems[i].Title, HintWrap = true },
                                    new AdaptiveText() { Text = viewModel.AllItems[i].description, HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        }
                    }
                };
                TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(content.GetXml()));
            }
            //如果没有item对磁贴进行初始处理
            if (viewModel.AllItems.Count == 0)
            {
                content = new TileContent()
                {
                    Visual = new TileVisual()
                    {
                        Branding = TileBranding.NameAndLogo,
                        TileSmall = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = "Title", HintWrap = true},
                                    new AdaptiveText() { Text = "Detail", HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        },

                        TileMedium = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = "This is the title", HintWrap = true },
                                    new AdaptiveText() { Text = "This is the description", HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        },

                        TileWide = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = "This is the title", HintWrap = true },
                                    new AdaptiveText() { Text = "This is the description", HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        },

                        TileLarge = new TileBinding()
                        {
                            Content = new TileBindingContentAdaptive()
                            {
                                BackgroundImage = new TileBackgroundImage()
                                {
                                    Source = "Assets/backgroud.jpg"
                                },

                                Children =
                                {
                                    new AdaptiveText() { Text = "This is the title", HintWrap = true },
                                    new AdaptiveText() { Text = "This is the description", HintStyle = AdaptiveTextStyle.CaptionSubtle, HintWrap = true}
                                }
                            }
                        }
                    }
                };
                var xmlDocument = content.GetXml();
                TileNotification tileNotification = new TileNotification(xmlDocument);
                TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
                tileUpdater.Update(tileNotification);
            }
        }
    }
}
