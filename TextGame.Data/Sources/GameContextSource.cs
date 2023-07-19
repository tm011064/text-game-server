using TextGame.Data.Contracts;

namespace TextGame.Data.Sources
{
    public class GameContextSource : IGameContextSource
    {
        private readonly IGameContextItemJsonSource<TerminalCommand[]> terminalCommandSource;

        private readonly IGameContextItemJsonSource<Emotion[]> emotionSource;

        private readonly IGameContextItemJsonSource<Chapter[]> chapterSource;

        public GameContextSource(
            IGameContextItemJsonSource<TerminalCommand[]> terminalCommandSource,
            IGameContextItemJsonSource<Emotion[]> emotionSource,
            IGameContextItemJsonSource<Chapter[]> chapterSource)
        {
            this.terminalCommandSource = terminalCommandSource;
            this.emotionSource = emotionSource;
            this.chapterSource = chapterSource;
        }

        public GameContext Get(string gameKey, string locale)
        {
            return new GameContext(
                chapterSource.Get(gameKey, locale),
                emotionSource.Get(gameKey, locale),
                terminalCommandSource.Get(gameKey, locale));
        }
    }

}

