using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day2
{
    class instagramUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
    class instagramPost
    {
        public int PostId { get; set; }
        public string Caption { get; set; }
        public int Likes { get; set; }
    }
    internal class InstagramApp
    {
        private static int postId = 1;
        private static int userId = 1;
        static instagramUser[] InstagramUser = new instagramUser[20];
        static instagramPost[][] InstagramPosts = new instagramPost[20][];
        private static void CreateNewUser()
        {
            Console.Write("Please enter the user name: ");
            string userName = Program.GetStringInput();
            Console.Write("Please enter number of posts: ");
            int postCount = Program.GetNumberInput();

            instagramPost[] posts = new instagramPost[postCount];
            instagramUser user = new instagramUser();
            user.UserName = userName;
            user.UserId = userId;
            for (int i = 0; i < postCount; i++)
            {
                Console.Write($"Please enter the caption for post {i + 1}: ");
                string caption = Program.GetStringInput();
                Console.Write($"Please enter the number of likes for post {i + 1}: ");
                int likes = Program.GetNumberInput();
                posts[i] = new instagramPost { PostId = postId++, Caption = caption, Likes = likes };
            }
            InstagramUser[userId-1] = user;
            InstagramPosts[userId++ - 1] = posts;
        }
        private static void DisplayAllUser()
        {
            bool hasUsers = false;
            Console.WriteLine("Displaying all Instagram users:");
            Console.WriteLine("Number of users: " + userId);
            for(int i=0;i<userId; i++)
            {
                if(InstagramUser[i] == null) continue;
                hasUsers = true;
                Console.WriteLine($"User ID: {InstagramUser[i].UserId}, User Name: {InstagramUser[i].UserName}");
            }
            if (!hasUsers)
            {
                Console.WriteLine("No users found.");
            }
        }
        private static void DisplayUserPosts(int userId)
        {
            foreach(var post in InstagramPosts)
            {
                if (post != null && post[0].PostId == userId)
                {
                    Console.WriteLine($"Displaying posts for User ID: {userId}");
                    foreach (var p in post)
                    {
                        Console.WriteLine($"Post ID: {p.PostId}, Caption: {p.Caption}, Likes: {p.Likes}");
                    }
                    return;
                }
            }
        }
        public static void Run()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("------------Welcome to Instagram App!----------");
                    Console.WriteLine("1. Create new User");
                    Console.WriteLine("2. Display Instagram users");
                    Console.WriteLine("3. Display Instagram user's Post");
                    Console.WriteLine("4. Exit the app");

                    Console.Write("Please enter your choice: ");
                    int choice = Program.GetNumberInput();

                    switch (choice)
                    {
                        case 1:
                            CreateNewUser();
                            break;
                        case 2:
                            DisplayAllUser();
                            break;
                        case 3:
                            Console.Write("Enter the user_id to display posts: ");
                            int userId = Program.GetNumberInput();
                            DisplayUserPosts(userId);
                            break;
                        case 4:
                            // Exit the app
                            Console.WriteLine("Exiting the app...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("---------Thank You!!Instagram has been closed!----------");
            }
        }
    }
}
