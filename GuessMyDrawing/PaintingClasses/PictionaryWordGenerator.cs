using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfekVentura_Project
{
    public class PictionaryWordGenerator
    {
        private static Random random = new Random();
        private static string[] wordList = { "Pac Man", "Rainbow", "Apple", "Chest", "Six pack", "Nail", "Tornado", "Mickey Mouse", "Youtube", "Lightning", "Traffic light", "Waterfall", "McDonalds", "Donald Trump", "Patrick", "Stop sign", "Superman", "Tooth", "Sunflower", "Keyboard", "Island", "Pikachu", "Harry Potter", "Nintendo Switch", "Facebook", "Eyebrow", "Peppa Pig", "SpongeBob", "Creeper", "Octopus", "Church", "Eiffel tower", "Tongue", "Snowflake", "Fish", "Twitter", "Pan", "Jesus Christ", "Butt cheeks", "Jail", "Pepsi", "Hospital", "Pregnant", "Thunderstorm", "Smile", "Skull", "Flower", "Palm tree", "Angry Birds", "America", "Lips", "Cloud", "Compass", "Mustache", "Captain America", "Pimple", "Easter Bunny", "Chicken", "Elmo", "Watch", "Prison", "Skeleton", "Arrow", "Volcano", "Minion", "School", "Tie", "Lighthouse", "Fountain", "Cookie Monster", "Iron Man", "Santa", "Blood", "River", "Bar", "Mount Everest", "Chest hair", "Gumball", "North", "Water", "Cactus", "Treehouse", "Bridge", "Short", "Thumb", "Beach", "Mountain", "Nike", "Flag", "Paris", "Eyelash", "Shrek", "Brain", "Iceberg", "Fingernail", "Playground", "Ice cream", "Google", "Dead", "Knife", "Spoon", "Unibrow", "Spiderman", "Black", "Graveyard", "Elbow", "Golden egg", "Yellow", "Germany", "Adidas", "Nose hair", "Deadpool", "Homer Simpson", "Bart Simpson", "Rainbow", "Ruler", "Building", "Raindrop", "Storm", "Coffee shop", "Windmill", "Fidget spinner", "Yo yo", "Ice", "Legs", "Tent", "Mouth", "Ocean", "Fanta", "Homeless", "Tablet", "Muscle", "Pinocchio", "Tear", "Nose", "Snow", "Nostrils", "Olaf", "Belly button", "Lion King", "Car wash", "Egypt", "Statue of Liberty", "Hello Kitty", "Pinky", "Winnie the Pooh", "Guitar", "Hulk", "Grinch", "Nutella", "Cold", "Flagpole", "Canada", "Rainforest", "Blue", "Rose", "Tree", "Hotmail", "Box", "Nemo", "Crab", "Knee", "Dog", "House", "Chrome", "Cotton candy", "Barack Obama", "Hot chocolate", "Michael Jackson", "Map", "Samsung", "Shoulder", "Microsoft", "Parking", "Forest", "Full moon", "Cherry blossom", "Apple", "seed", "Donald Duck", "Leaf", "Batear", "Wax", "Italy", "Finger", "Seed", "Lilypad", "Brush", "Record", "Wrist", "Thunder", "Gummy", "Kirby", "Fire hydrant", "Overweight", "Hot dog", "House", "Fork", "Pink", "Sonic", "Street", "NASA", "Arm", "Fast", "Tunnel", "Full", "Library", "Pet shop", "Yoshi", "Russia", "Drum kit", "Android", "Finn and Jake", "Price tag", "Tooth Fairy", "Bus stop", "Rain", "Heart", "Face", "Tower", "Bank", "Cheeks", "Batman", "Speaker", "Thor", "Skinny", "Electric guitar", "Belly", "Cute", "Ice cream truck", "Bubble gum", "Top hat", "Pink Panther", "Hand", "Bald", "Freckles", "Clover", "Armpit", "Japan", "Thin", "Traffic", "Spaghetti", "Phineas and Ferb", "Broken heart", "Fingertip", "Funny", "Poisonous", "Wonder Woman", "Squidward", "Mark Zuckerberg", "Twig", "Red", "China", "Dream", "Dorado", "Daisy", "France", "Discord", "Toenail", "Positive", "Forehead", "Earthquake", "Iron", "Zeus", "Mercedes", "Big Ben", "Supermarket", "Bugs Bunny", "Yin and Yang", "Drink", "Rock", "Drum", "Piano", "White", "Bench", "Fall", "Royal", "Seashell", "Audi", "Stomach", "Aquarium", "Bitcoin", "Volleyball", "Marshmallow", "Cat Woman", "Underground", "Green Lantern", "Bottle flip", "Toothbrush", "Globe", "Sand", "Zoo", "West", "Puddle", "Lobster", "North Korea", "Luigi", "Bamboo", "Great Wall", "Kim Jong un", "Bad", "Credit card", "Swimming pool", "Wolverine", "Head", "Hair", "Yoda", "Elsa", "Turkey", "Heel", "Maracas", "Cleandrop", "Let", "Cinema", "Poor", "Stamp", "Africa", "Whistle", "Teletubby", "Wind", "Aladdin", "Tissue box", "Fire truck", "Usain Bolt", "Water gun", "Farm", "iPad", "Well", "Warm", "Booger", "WhatsApp", "Skype", "Landscape", "Pine cone", "Mexico", "Slow", "Organ", "Fish bowl", "Teddy bear", "John Cena", "Frankenstein", "Tennis racket", "Gummy bear", "Mount Rushmore", "Swing", "Mario", "Lake", "Point", "Vein", "Cave", "Smell", "China", "Desert", "Scary", "Dracula", "Airport", "Kiwi", "Seaweed", "Incognito", "Pluto", "Statue", "Hairy", "Strawberry", "Low", "Invisible", "Blindfold", "Tuna", "Controller", "Paypal" };
        // This method returns a list of 3 unique random words from a predefined word list.
        public static List<string> GetRandomWords()
        {
            List<string> randomWords = new List<string>();
            HashSet<int> indexes = new HashSet<int>();
            while (indexes.Count < 3)
            {
                int index = random.Next(wordList.Length);
                if (!indexes.Contains(index))
                {
                    indexes.Add(index);
                    randomWords.Add(wordList[index].ToUpper());
                }
            }
            return randomWords;
        }
    }
}