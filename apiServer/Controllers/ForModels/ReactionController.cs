﻿using apiServer.Controllers.Authentication;
using apiServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiServer.Controllers.ForModels
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        private readonly ArhivistDbContext _context;

        public ReactionController(ArhivistDbContext context, ILogger<EmotionController> logger, TokensController tokens)
        {
            _context = context;
        }

        [HttpPost("AddReaction")]
        public ActionResult AddReaction(Reactions reaction)
        {
            //reaction.article_id = "fec4ce59-2ce9-4398-a2a1-e36af2795af7";
            //reaction.user_id = "c56274f1-8311-4172-9847-d62376494bc2";
            //reaction.reaction_id = "1";
            //Пример

            reaction.Id = Guid.NewGuid().ToString();
            reaction.date_create = DateTime.Now;
            reaction.modified_date = DateTime.Now;
            _context.Reactions.Add(reaction);
            _context.SaveChanges();
            return Ok("Реакция добавленна");
        }
        [HttpGet("GetReactionForArticle")]
        public ActionResult GetReactionForArticle(string articleId)
        {
            int CountReaction = _context.Reactions.Count(r => r.article_id == articleId);
            return Ok(CountReaction);
        }
    }
}
