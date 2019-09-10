﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QA.Domain.Entities;
using QA.Domain.Services;
using QA.Domain.Commands;

namespace QA.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostQueryService _postQueryService;
        private readonly IPostCommandService _postCommandService;

        public PostController(IPostQueryService postQueryService, IPostCommandService postCommandService)
        {
            _postQueryService = postQueryService;
            _postCommandService = postCommandService;
        }

        // GET: api/Post
        [HttpGet]
        public IEnumerable<Question> Get(string searchTerm, int page, int count)
        {
            var questions = _postQueryService.GetQuestions(searchTerm);
            return questions.Skip(count * page).Take(count).ToArray();
        }        

        //GET: api/Post/id
        [HttpGet]
        [Route("{questionId}")]
        public Question Get(string questionId)
        {
            return _postQueryService.GetQuestion(Guid.Parse(questionId));
        }

        [HttpPost]
        [Route("{questionId}/vote")]
        public Question VoteQuestion(string questionId, [FromBody] bool up)
        {
            //TODO: fix after authentication
            var user = new User { Id = Guid.NewGuid(), Username = ".." };

            var result = _postCommandService.Execute(new VoteQuestionCommand(user, Guid.Parse(questionId), up ? Direction.Up : Direction.Down));
            if (result.IsSuccessful) return result.Entity as Question;
            else return null;
        }

        [HttpPost]
        [Route("{questionId}/comment")]
        public Comment AddQuestionComment(string questionId, [FromBody] string text)
        {
            //TODO: fix after authentication
            var user = new User { Id = Guid.NewGuid(), Username = ".." };

            var result = _postCommandService.Execute(new CreateCommentCommand(user, text, Guid.Parse(questionId)));
            if (result.IsSuccessful) return result.Entity as Comment;
            else return null;
        }

        [HttpPost]
        [Route("{questionId}/{answerId}/accept")]
        public void AcceptAnswer(string questionId, string answerId)
        {
            //TODO: fix after authentication
            var user = new User { Id = Guid.NewGuid(), Username = ".." };

            var result = _postCommandService.Execute(new AcceptAnswerCommand(user, Guid.Parse(answerId), true));
        }

        [HttpPost]
        [Route("{questionId}/{answerId}/vote")]
        public Answer VoteAnswer(string questionId, string answerId, [FromBody] bool up)
        {
            //TODO: fix after authentication
            var user = new User { Id = Guid.NewGuid(), Username = ".." };

            var result = _postCommandService.Execute(new VoteAnswerCommand(user, Guid.Parse(answerId), up ? Direction.Up : Direction.Down));
            if (result.IsSuccessful) return result.Entity as Answer;
            else return null;
        }

        [HttpPost]
        [Route("{questionId}/answer")]
        public Answer AddAnswer(string questionId, [FromBody]string answer)
        {
            //TODO: fix after authentication
            var user = new User { Id = Guid.NewGuid(), Username = ".." };

            var result = _postCommandService.Execute(new CreateAnswerCommand(user, answer, Guid.Parse(questionId)));
            if (result.IsSuccessful) return result.Entity as Answer;
            else return null;
        }
    }
}
