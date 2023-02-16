using Entities;
using IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class IndexModel : PageModel
    {
        public IDbManager DbManager = new DbManagerSql();
        public int? GameId { get; set; }
        public GameState? LatestState { get; set; }
        
        public GameState? HistoryStateToUse { get; set; }

        private bool _redirected;
        
        [BindProperty]
        public string? MoveFrom { get; set; }
        [BindProperty]
        public string? MoveTo { get; set; }
        [BindProperty]
        public string? Origin { get; set; }
        
        [BindProperty]
        public string? DimensionsField { get; set; }
        [BindProperty]
        public string? WhiteMovesFirstField { get; set; }
        [BindProperty]
        public string? GameTypeField { get; set; }

        private void RedirectWithError(string errMsg)
        {
            if (errMsg.Length > 0) Response.Redirect(Request.Path + $"?errMsg={errMsg}");
            else Response.Redirect(Request.Path);
            _redirected = true;
        }

        private void MakeNewGame()
        {
            if (DimensionsField == null || GameTypeField == null)
            {
                RedirectWithError("All the fields must be covered.");
                return;
            }
            var options = new GameOptions(
                int.Parse(DimensionsField),
                WhiteMovesFirstField != null,
                new [] { EGameType.HumanVsHuman, EGameType.HumanVsAi,
                    EGameType.AiVsAi }[int.Parse(GameTypeField)]
            );
            var state = DbManager.MakeBrandNewState(options);
            state = DbManager.SaveState(state);
            Response.Redirect($"Game/{state.Game.Id}");
        }

        private void AttemptMakeMove()
        {
            if (Origin == null)
            {
                RedirectWithError("Something strange went wrong. 1");
                return;
            }

            var state = DbManager.LoadStateFromNum(int.Parse(Origin));
            if (state == null)
            {
                RedirectWithError("Something strange went wrong. 2");
                return;
            }
                
            if (MoveFrom == null || MoveTo == null)
            {
                var aiState = state.MakeRandomMove();
                if (aiState != null) DbManager.SaveState(aiState);
                return;
            }
                
            var fromSq = Square.FromInput(MoveFrom);
            var toSq = Square.FromInput(MoveTo);
            if (fromSq == null || toSq == null)
            {
                RedirectWithError("Bad input.");
                return;
            }

            try
            {
                DbManager.SaveState(state.MakeMove(fromSq, toSq));
            }
            catch (ArgumentException e)
            {
                RedirectWithError(e.Message);
            }
        }
        
        public void OnPost()
        {
            foreach (var arg in new [] {MoveFrom, MoveTo, Origin, DimensionsField,
                         WhiteMovesFirstField, GameTypeField}) Console.WriteLine(arg);
            
            if (Origin is "new") MakeNewGame();
            else AttemptMakeMove();
            
            if (!_redirected) RedirectWithError("");
        }
    }
}
