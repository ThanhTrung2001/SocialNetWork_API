﻿namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NewPostBasic
    {
        public bool IsNotification { get; set; }
        public string PostType { get; set; }
        public string PostDestination { get; set; }
        public string Content { get; set; }
    }

    public class CreatePostRequest
    {
        public string UserId { get; set; }
        public NewPostBasic NewPost { get; set; }
    }
}
