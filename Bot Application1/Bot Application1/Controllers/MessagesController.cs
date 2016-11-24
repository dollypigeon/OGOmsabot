using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.Text.RegularExpressions;
using Bot_Application1.Datamodels;
using System.Collections.Generic;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            // check if activity is of type message
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {               
                await Conversation.SendAsync(activity, () => new EchoDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }


        [Serializable]
    public class EchoDialog : IDialog<object>
    {
        public string RidesOption="";
        public string DateOption="";
        public string TimeOption="";
        public string Namestr="";
        public string Agestr="";

        public string Phonestr="";
        public string userchoice = "";
        public double totalprice;
        public int count = 0;

            public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

            private static Attachment GetHeroCard()
            {
                var heroCard = new HeroCard
                {
                    Title = "OGO Rotorua NZ",
                    Text = "Book your rides here to start your adventure.",
                    Images = new List<CardImage> { new CardImage("http://www.ogo.co.nz/images/4.jpg") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "View our website", value: "http://www.ogo.co.nz/") }
                };
                return heroCard.ToAttachment();
            }
            public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

                var message = await argument;
                var message1 = context.MakeMessage();
                var attachment = GetHeroCard();
                message1.Attachments = new List<Attachment>();
                message1.Attachments.Add(attachment);
                await context.PostAsync(message1);
                HttpClient client = new HttpClient();
                string x = await client.GetStringAsync(new Uri("http://api.openweathermap.org/data/2.5/weather?q=Rotorua" + "&units=metric&APPID=440e3d0ee33a977c5e2fff6bc12448ee"));
                WeatherObject.RootObject rootObject;
                rootObject = JsonConvert.DeserializeObject<WeatherObject.RootObject>(x);
                string temp = rootObject.main.temp + "°C";
                if (count == 0)
                {
                    count += 1;
                    await context.PostAsync("Welcome! The current temperature for Rotorua is " + temp + ".     \n    We have two kinds of rides with each has two options, which one would you like to book?     \n     a. H2OGO single person-$45    \n    b. H2OGO two person-$80    \n    c. DRYGO single person-$45    \n    d. DRYGO two person-$80");
                    context.Wait(RidesReceivedAsync);
                }
                else
                {
                    await context.PostAsync("Welcome back! The current temperature for Rotorua is " + temp + ".     \n    We have two kinds of rides with each has two options, which one would you like to book?     \n     a. H2OGO single person-$45    \n    b. H2OGO two person-$80    \n    c. DRYGO single person-$45    \n    d. DRYGO two person-$80");
                    context.Wait(RidesReceivedAsync);
                }
               
        }

        public async Task RidesReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
                List<Rides> rides = await AzureManager.AzureManagerInstance.GetRides();

                var userinput = await argument;
            if (userinput.Text == "a")
            {
                this.RidesOption = "H2O1";
                    foreach (var p in rides)
                    {
                        if (p.TYPE == this.RidesOption)
                        {
                            totalprice += double.Parse(p.PRICE);
                            userchoice =userchoice+ p.TYPE+"  ";
                        }
                    }
                    await context.PostAsync("Do you want to book anything else?");
                context.Wait(OtherOptionReceivedAsync);
            }
            else if (userinput.Text == "b")
            {
                this.RidesOption = "H2O2";
                    foreach (var p in rides)
                    {
                        if (p.TYPE == this.RidesOption)
                        {
                            totalprice += double.Parse(p.PRICE);
                            userchoice = userchoice + p.TYPE + "  ";
                        }
                    }
                    await context.PostAsync("Do you want to book anything else?");
                context.Wait(OtherOptionReceivedAsync);
            }
            else if (userinput.Text == "c")
            {
                this.RidesOption = "DRY1";
                    foreach (var p in rides)
                    {
                        if (p.TYPE == this.RidesOption)
                        {
                            totalprice += double.Parse(p.PRICE);
                            userchoice = userchoice + p.TYPE + "  ";
                        }
                    }
                    await context.PostAsync("Do you want to book anything else?");
                context.Wait(OtherOptionReceivedAsync);
            }
            else if (userinput.Text == "d")
            {
                this.RidesOption = "DRY2";
                    foreach (var p in rides)
                    {
                        if (p.TYPE == this.RidesOption)
                        {
                            totalprice += double.Parse(p.PRICE);
                            userchoice = userchoice + p.TYPE + "  ";
                        }
                    }
                    await context.PostAsync("Do you want to book anything else?");
                context.Wait(OtherOptionReceivedAsync);
            }
            else
            {
                await context.PostAsync("Please enter valid input: a, b, c, d");
                context.Wait(RidesReceivedAsync);
            }
        }

            public async Task OtherOptionReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var option = await argument;
                if (option.Text.ToLower() == "yes")
                {
                    await context.PostAsync("We have two kinds of rides with each has two options, which one do you want to book?     \n     a. H2OGO single person    \n    b. H2OGO two person.    \n    c. DRYGO single person.    \n    d. DRYGO two person.");
                    context.Wait(RidesReceivedAsync);
                }
                else if (option.Text.ToLower()=="no")
                {
                    await context.PostAsync("What date would you like to book? eg. yyyy mm dd (please seperate by space)");
                    context.Wait(DateReceivedAsync);
                }
                else
                {
                    await context.PostAsync("Please reply yes or no.");
                    context.Wait(OtherOptionReceivedAsync);
                }
            }

            public async Task DateReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var inputlist = message.Text.Split();
            DateOption = "";
            foreach (var x in inputlist)
            {
                DateOption = DateOption + x + "/";

            }
                DateOption = DateOption.Substring(0, DateOption.Length - 1);
                DateTime d1 = new DateTime(int.Parse(inputlist[0]), int.Parse(inputlist[1]), int.Parse(inputlist[2]));
            if (d1 > DateTime.Today.Date)
            {
                await context.PostAsync("We open from 9am to 6pm. What time would you like to book? eg. 09 00 ");
                context.Wait(TimeReceivedAsync);
            }
            else
            {
                await context.PostAsync("Please enter valid date.");
                context.Wait(DateReceivedAsync);
            }
        }

        public async Task TimeReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var time = await argument;
            var inputlist = time.Text.Split();
            if (int.Parse(inputlist[0]) >= 9 && int.Parse(inputlist[0]) < 18)
            {
                TimeOption = inputlist[0];
                await context.PostAsync("May I have your name please? Please seperate your forename and surname by space.");
                context.Wait(NameReceivedAsync);
            }
            else
            {
                await context.PostAsync("Our opening hour: 9am-6pm. Please enter valid time. eg. 09 00");
                context.Wait(TimeReceivedAsync);
            }
        }

        public async Task NameReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var name = await argument;
            var inputlist = name.Text.Split();
                Namestr = "";
            foreach (var x in inputlist)
            {
                Namestr = Namestr + x + " ";
            }

            Regex regex = new Regex(@"^[a-zA-Z\\s]+");
            if (regex.IsMatch(Namestr)==true)
            {
                await context.PostAsync("Please enter your age. eg.18");
                context.Wait(AgeReceivedAsync);
            }
            else
            {
                await context.PostAsync("Please enter valid name.");
                context.Wait(NameReceivedAsync);
            }
        }

        public async Task AgeReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var age = await argument;
            if (int.Parse(age.Text)>=6 && int.Parse(age.Text) <= 100)
            {
                Agestr = age.Text;
                await context.PostAsync("Please enter your phone number. eg. 021-1234567");
                context.Wait(PhoneReceivedAsync);
            }
            else
            {
                await context.PostAsync("Please enter valid number between 6-90. eg.18");
                context.Wait(AgeReceivedAsync);
            }
        }

            private Attachment GetReceiptCard()
            {
                var receiptCard = new ReceiptCard
                {
                    Title = Namestr,
                    Facts = new List<Fact> {new Fact("Phone", Phonestr), new Fact("Date", DateOption), new Fact("Time", TimeOption + ":00") },
                    Items = new List<ReceiptItem>
        {
            new ReceiptItem("Your rides",userchoice),
        },
                    Total = "$" + totalprice.ToString(),
                };

                return receiptCard.ToAttachment();
            }


            public async Task PhoneReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var phone = await argument;
            Regex regex = new Regex("^[0-9]+(-[0-9]+)*$");
            if (regex.IsMatch(phone.Text)==true)
            {
                    Phonestr = phone.Text;
                    Customer Cname = new Customer()
                    {
                        CNAME = Namestr,
                        CAGE = Agestr,
                        PHONE=Phonestr,
                        DBDATE=DateOption,
                        TIME=TimeOption
                    };
                    await AzureManager.AzureManagerInstance.AddCustomer(Cname);

                    await context.PostAsync("Please confirm your booking information showing below by replying Yes or Cancel.");
                    var message1 = context.MakeMessage();
                    var attachment = GetReceiptCard();
                    message1.Attachments = new List<Attachment>();
                    message1.Attachments.Add(attachment);
                    await context.PostAsync(message1);

                    context.Wait(ConfirmReceivedAsync);
            }
            else
            {
                await context.PostAsync("Please enter valid number. eg. 021-1234567");
                context.Wait(PhoneReceivedAsync);
            }
        }

 

            private static Attachment GetThumbnailCard()
            {
                var heroCard2 = new ThumbnailCard
                {
                    Title = "Thanks for your booking!",
                    Subtitle = "Our address is 525 Ngongotaha Road, Fairy Springs, Rotorua, New Zealand.",
                    Images = new List<CardImage> { new CardImage("http://www.ogo.co.nz/images/logo.png") },
                };

                return heroCard2.ToAttachment();
            }
        
            public async Task ConfirmReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var confirmation = await argument;
                var userid = "";
                var message2 = context.MakeMessage();
                var attachment = GetThumbnailCard();
                message2.Attachments = new List<Attachment>();
                message2.Attachments.Add(attachment);
               
                if (confirmation.Text.ToLower()=="yes")
                {
                    DateOption = DateOption.Substring(0, DateOption.Length - 1);
                    await context.PostAsync(message2);
                    userchoice = "";
                    totalprice = 0;
                    context.Wait(MessageReceivedAsync);
                }
                else if (confirmation.Text.ToLower()=="cancel")
                {                   
                    List<Customer> customers = await AzureManager.AzureManagerInstance.GetCustomers();
                    foreach (var i in customers)
                    {
                        if (i.CNAME == Namestr && i.PHONE == Phonestr)
                        {
                            userid = i.ID;
                        }
                    }
                    Customer deletecus = new Customer()
                    {
                        ID = userid
                    };
                    await AzureManager.AzureManagerInstance.DeleteCustomer(deletecus);
                    await context.PostAsync("Your booking has been canceled, reply anything to start again.");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync("Please enter valid input.");
                    context.Wait(ConfirmReceivedAsync);
                }
            }
        }



        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}