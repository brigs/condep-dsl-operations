﻿using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Remote.Node;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Application.Deployment.CopyDir
{
    public class CopyDirOperation : ForEachServerOperation
    {
        private readonly string _srcDir;
        private readonly string _dstDir;
        private Api _api;

        public CopyDirOperation(string srcDir, string dstDir)
        {
            _srcDir = srcDir;
            _dstDir = dstDir;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            _api = new Api(new ConDepNodeUrl(server, settings), server.DeploymentUser.UserName, server.DeploymentUser.Password, server.Node.TimeoutInSeconds.Value * 1000);
            var result = _api.SyncDir(_srcDir, _dstDir);

            if (result == null) return;
            
            if(result.Log.Count > 0)
            {
                foreach (var entry in result.Log)
                {
                    Logger.Info(entry);
                }
            }
            else
            {
                Logger.Info("Nothing to deploy. Everything is in sync.");
            }
        }

        public override string Name { get { return "Copy Dir"; } }
        public void DryRun()
        {
            Logger.WithLogSection(Name, () => { });
        }
    }
}