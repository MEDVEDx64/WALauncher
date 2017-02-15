using System;
using System.Collections.Generic;

namespace WALauncher.Wapkg
{
    public class ServiceMessageEventArgs : EventArgs
    {
        public string RawMessage { get; }

        public string RelatedDistribution { get; }
        public IReadOnlyList<Tuple<string, uint?, string>> Packages { get; }
        public IReadOnlyList<string> Distributions { get; }
        public IReadOnlyList<string> Sources { get; }

        public string ActionToken { get; }
        public int ActionProgressCurrent { get; }
        public int ActionProgressTotal { get; }

        public string Text { get; }

        public ServiceMessageEventArgs(string raw)
        {
            RawMessage = raw;
        }

        public ServiceMessageEventArgs(string raw, string text)
        {
            RawMessage = raw;
            Text = text;
        }

        public ServiceMessageEventArgs(string raw, string distro, IReadOnlyList<Tuple<string, uint?, string>> packages)
        {
            RawMessage = raw;
            RelatedDistribution = distro;
            Packages = packages;
        }

        public ServiceMessageEventArgs(string raw, IReadOnlyList<string> dists)
        {
            RawMessage = raw;
            Distributions = dists;
        }

        public ServiceMessageEventArgs(string raw, IReadOnlyList<string> dists, IReadOnlyList<string> sources)
        {
            RawMessage = raw;
            Distributions = dists;
            Sources = sources;
        }

        public ServiceMessageEventArgs(string raw, string actionToken, int current, int total)
        {
            ActionToken = actionToken;
            ActionProgressCurrent = current;
            ActionProgressTotal = total;
        }
    }
}
