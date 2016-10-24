using System.Collections;
using System.Collections.Generic;
using WALauncher.Utils;
using WALauncher.Wapkg;

namespace WALauncher.ViewModels.Sources
{
    public class SourceDefinition : ViewModelBase
    {
        List<string> sources = new List<string>(); // local copy
        InteractionService wapkg = InteractionService.Get();

        public string Link { get; }

        public DelegateCommand MoveUpCommand { get; }
        public DelegateCommand MoveDownCommand { get; }
        public DelegateCommand RemoveCommand { get; }

        public SourceDefinition(string link)
        {
            Link = link;
            
            MoveUpCommand = new DelegateCommand(MoveUp);
            MoveDownCommand = new DelegateCommand(MoveDown);
            RemoveCommand = new DelegateCommand(Remove);
        }

        public void ApplySources(IEnumerable sources)
        {
            foreach (var src in sources)
            {
                this.sources.Add(src.ToString());
            }
        }

        public override string ToString()
        {
            return Link;
        }

        void MoveUp()
        {
            var index = sources.IndexOf(Link);
            if (index > 0)
            {
                sources.Remove(Link);
                sources.Insert(index - 1, Link);
                wapkg.PushSources(sources);
            }
        }

        void MoveDown()
        {
            var index = sources.IndexOf(Link);
            if(index >= 0 && index < sources.Count - 1)
            {
                sources.Remove(Link);
                sources.Insert(index + 1, Link);
                wapkg.PushSources(sources);
            }
        }

        void Remove()
        {
            if(sources.Contains(Link))
            {
                sources.Remove(Link);
                wapkg.PushSources(sources);
            }
        }
    }
}
