using System.Collections;
using System.Collections.Generic;
using WALauncher.Utils;

namespace WALauncher.ViewModels.Sources
{
    public class SourceDefinition : ViewModelBase
    {
        List<string> sources = new List<string>(); // local copy

        public string Link { get; }

        public DelegateCommand MoveUpCommand { get; }
        public DelegateCommand MoveDownCommand { get; }
        public DelegateCommand RemoveCommand { get; }

        public SourceDefinition(string link, IEnumerable sources)
        {
            Link = link;
            foreach(var src in sources)
            {
                this.sources.Add(src.ToString());
            }
        }

        public override string ToString()
        {
            return Link;
        }
    }
}
