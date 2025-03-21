﻿namespace EV.Model.Handlers.Commands
{
    public class CreatePostReactCommand
    {
        public Guid Post_Id { get; set; }
        public string React_Type { get; set; }
        public bool Is_SharePost { get; set; }
        public Guid User_Id { get; set; }
    }

    public class CreateCommentReactCommand
    {
        public Guid Comment_Id { get; set; }
        public string React_Type { get; set; }
        public Guid User_Id { get; set; }
    }

    public class CreateMessageReactCommand
    {
        public Guid Message_Id { get; set; }
        public string React_Type { get; set; }
        public Guid User_Id { get; set; }
    }

    public class EditReactCommand
    {
        public string React_Type { get; set; }
        public Guid User_Id { get; set; }
    }

    public class DeletePostReactCommand
    {
        public Guid User_Id { get; set; }
        public bool Is_SharePost { get; set; }
    }

    public class DeleteReactCommand
    {
        public Guid User_Id { get; set; }
        public bool Is_SharePost { get; set; }
    }
}