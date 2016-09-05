using System;
using System.Collections.Generic;

namespace WALauncher.Wapkg
{
    public class ServiceMessageEventArgs : EventArgs
    {
        public string RawMessage { get; private set; }

        public string RelatedDistribution { get; private set; }
        public IReadOnlyList<Tuple<string, uint?>> Packages { get; private set; }
        public IReadOnlyList<string> Distributions { get; private set; }

        public string Text { get; private set; }

        public ServiceMessageEventArgs(string raw)
        {
            RawMessage = raw;
        }

        public ServiceMessageEventArgs(string raw, string text)
        {
            RawMessage = raw;
            Text = text;
        }

        public ServiceMessageEventArgs(string raw, string distro, IReadOnlyList<Tuple<string, uint?>> packages)
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
    }
}
