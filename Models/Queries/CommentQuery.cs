﻿namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class CommentQuery
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool IsResponse { get; set; }
        public int ReactCount { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Avatar { get; set; }
    }

    public class CommentDetailQuery
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool IsResponse { get; set; }
        public int ReactCount { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Avatar { get; set; }
        public List<CommentReactQuery>? Reacts { get; set; } = new List<CommentReactQuery>();
        public List<CommentDetailQuery>? Responses { get; set; } = new List<CommentDetailQuery>();
    }
}
