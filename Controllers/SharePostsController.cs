﻿using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Entities.Commands;
using EnVietSocialNetWorkAPI.Entities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace EnVietSocialNetWorkAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SharePostsController : ControllerBase
    {
        private readonly DapperContext _context;

        public SharePostsController(DapperContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<SharePostQuery>> Get()
        {
            var query = @"SELECT 
                            s.Id,
                            s.Content AS ShareContent,
                            s.CreatedAt AS ShareCreatedAt,
                            us.Id AS ShareUserId,
                            us.UserName AS ShareUserName,
                            us.AvatarUrl AS ShareUserAvatar,
                            p.Id AS PostId,
                            p.Content AS PostContent,
                            p.PostType,
                            p.CreatedAt,
                            p.PostDestination,
                            up.Id AS UserId,
                            up.UserName,
                            up.Email,
                            up.AvatarUrl,
                            m.URL AS MediaUrl,
                            
                            su.Id AS SurveyId,
                            su.ExpiredIn,
                            su.Question AS SurveyQuestion,

                            si.Id AS SurveyItemId,
                            si.Content AS SurveyItemContent,
                            si.Votes AS SurveyItemVotes,

                            sv.VoteId,
                            sv.UserId AS VoteUserId,              
                            usv.UserName AS VoteUserName,
                            usv.AvatarUrl AS VoteUserAvatar

                         FROM SharePosts s
                         INNER JOIN Users us ON s.SharedByUserId = us.ID
                         LEFT JOIN Posts p ON p.Id = s.SharedPostId
                         LEFT JOIN 
                            Users up ON p.OwnerId = up.Id
                         LEFT JOIN
                            MediaItems m ON p.Id = m.PostId
                         LEFT JOIN 
                            Surveys su ON p.Id = su.PostId
                        LEFT JOIN 
                            SurveyItems si ON su.Id = si.SurveyId
                        LEFT JOIN
                            SurveyVotes sv ON si.Id = sv.OptionId
                        LEFT JOIN 
                            Users usv ON usv.Id = sv.UserId 
                        WHERE s.IsDeleted = 0";
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, string, PostSurveyQuery, SurveyItemQuery, SurveyItemVote, SharePostQuery>(
                    query,
                    map: (share, mediaUrl, survey, surveyItem, vote) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (!string.IsNullOrEmpty(mediaUrl) && !shareEntry.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            shareEntry.MediaUrls.Add(mediaUrl);
                        }

                        if (share.PostType == "survey" && survey != null)
                        {
                            share.Survey = survey;
                            if (surveyItem != null && !share.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                share.Survey.SurveyItems.Add(surveyItem);
                                var result = share.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.SurveyVotes.Any((item) => item.VoteId == vote.VoteId))
                                {
                                    result.SurveyVotes.Add(vote);
                                }
                            }
                        }

                        //if (comment != null && !shareEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        //{
                        //    shareEntry.Comments.Add(comment);
                        //}
                        //if (react != null && !shareEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        //{
                        //    shareEntry.Reacts.Add(react);
                        //}
                        return shareEntry;
                    },

                    splitOn: "MediaUrl, SurveyId, SurveyItemId, VoteId");
                    return shareDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IEnumerable<SharePostQuery>> GetByUserID(Guid id)
        {
            var query = @"SELECT 
                            s.Id,
                            s.Content AS ShareContent,
                            s.CreatedAt AS ShareCreatedAt,
                            us.Id AS ShareUserId,
                            us.UserName AS ShareUserName,
                            us.AvatarUrl AS ShareUserAvatar,
                            p.Id AS PostId,
                            p.Content AS PostContent,
                            p.PostType,
                            p.CreatedAt,
                            p.PostDestination,
                            up.Id AS UserId,
                            up.UserName,
                            up.Email,
                            up.AvatarUrl,
                            m.URL AS MediaUrl,
                            
                            s.Id AS SurveyId,
                            s.ExpiredIn,
                            s.Question AS SurveyQuestion,

                            si.Id AS SurveyItemId,
                            si.Content AS SurveyItemContent,
                            si.Votes AS SurveyItemVotes,

                            sv.VoteId,
                            sv.UserId AS VoteUserId,              
                            usv.UserName AS VoteUserName,
                            usv.AvatarUrl AS VoteUserAvatar

                         FROM SharePosts s
                         INNER JOIN Users us ON s.SharedByUserId = us.ID
                         LEFT JOIN Posts p ON p.Id = s.SharedPostId
                         LEFT JOIN 
                            Users up ON p.OwnerId = up.Id
                         LEFT JOIN
                            MediaItems m ON p.Id = m.PostId
                         LEFT JOIN 
                            Surveys su ON p.Id = su.PostId
                        LEFT JOIN 
                            SurveyItems si ON su.Id = si.SurveyId
                        LEFT JOIN
                            SurveyVotes sv ON si.Id = sv.OptionId
                        LEFT JOIN 
                            Users usv ON usv.Id = sv.UserId 
                        WHERE s.IsDeleted = 0 AND s.SharedByUserId = @Id";
            try
            {
                var shareDict = new Dictionary<Guid, SharePostQuery>();

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QueryAsync<SharePostQuery, string, PostSurveyQuery, SurveyItemQuery, SurveyItemVote, SharePostQuery>(
                    query,
                    map: (share, mediaUrl, survey, surveyItem, vote) =>
                    {
                        if (!shareDict.TryGetValue(share.Id, out var shareEntry))
                        {
                            shareEntry = share;
                            shareDict.Add(share.Id, shareEntry);
                        }

                        if (!string.IsNullOrEmpty(mediaUrl) && !shareEntry.MediaUrls.Any((item) => item == mediaUrl))
                        {
                            shareEntry.MediaUrls.Add(mediaUrl);
                        }

                        if (share.PostType == "survey" && survey != null)
                        {
                            share.Survey = survey;
                            if (surveyItem != null && !share.Survey.SurveyItems.Any((item) => item.SurveyItemId == surveyItem.SurveyItemId))
                            {
                                share.Survey.SurveyItems.Add(surveyItem);
                                var result = share.Survey.SurveyItems.FirstOrDefault((x) => x.SurveyItemId == surveyItem.SurveyItemId);
                                if (vote != null && !result.SurveyVotes.Any((item) => item.VoteId == vote.VoteId))
                                {
                                    result.SurveyVotes.Add(vote);
                                }
                            }
                        }

                        //if (comment != null && !shareEntry.Comments.Any((item) => item.CommentId == comment.CommentId))
                        //{
                        //    shareEntry.Comments.Add(comment);
                        //}
                        //if (react != null && !shareEntry.Reacts.Any((item) => item.ReactId == react.ReactId))
                        //{
                        //    shareEntry.Reacts.Add(react);
                        //}
                        return shareEntry;
                    },
                    new { Id = id },
                    splitOn: "MediaUrl, SurveyId, SurveyItemId, VoteId");
                    return shareDict.Values.ToList();
                }
            }
            catch
            {
                throw;
            }
        }



        [HttpGet("post/{postId}/users")]
        public async Task<IEnumerable<ShareUserQuery>> GetShareUsersByPostId(Guid postId)
        {
            var query = @" SELECT s.Id, s.Content AS ShareContent, u.Id AS ShareUserId, u.UserName AS ShareUserName, u.AvatarUrl as ShareUserAvatar
                           FROM SharePosts s
                           LEFT JOIN User u ON s.ShareByUserId = u.Id
                           WHERE s.SharePostId = @PostId";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<ShareUserQuery>(query, new { PostId = postId });
                return result;
            }
        }

        [HttpPost("post/{postId}")]
        public async Task<IActionResult> CreateSharePost(Guid postId, NewSharePost share)
        {
            var query = @"INSERT INTO SharePosts (Id, CreatedAt, UpdatedAt, IsDeleted, SharedPostId ,SharedByUserId, Content, TargetType, TargetId)
                          VALUES
                          (NEWID(), GETDATE(), GETDATE(), 0, @PostId, @SharedByUserId, @Content, @TargetType, @TargetId)";
            var parameter = new DynamicParameters();
            parameter.Add("PostId", postId, DbType.Guid);
            parameter.Add("SharedByUserId", share.UserId, DbType.Guid);
            parameter.Add("Content", share.Content, DbType.String);
            parameter.Add("TargetType", share.TargetType, DbType.String);
            parameter.Add("TargetId", share.TargetID, DbType.Guid);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameter);
                return Ok();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var query = "UPDATE SharePosts SET isDeleted = 1 WHERE Id = @Id;";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
                return Ok();
            }
        }
    }
}