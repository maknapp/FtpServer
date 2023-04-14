// <copyright file="FtpConnectionIdleCheck.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FubarDev.FtpServer.ConnectionChecks
{
    /// <summary>
    /// An activity-based keep-alive detection.
    /// </summary>
    public class FtpConnectionIdleCheck : IFtpConnectionCheck
    {
        /// <summary>
        /// The timeout for the detection of inactivity.
        /// </summary>
        private readonly TimeSpan _inactivityTimeout;

        private readonly ILogger<FtpConnectionIdleCheck> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpConnectionIdleCheck"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="options">FTP connection options.</param>
        public FtpConnectionIdleCheck(
            ILogger<FtpConnectionIdleCheck> logger,
            IOptions<FtpConnectionOptions> options)
        {
            _inactivityTimeout = options.Value.InactivityTimeout ?? TimeSpan.MaxValue;
            _logger = logger;
        }

        /// <inheritdoc />
        public FtpConnectionCheckResult Check(FtpConnectionCheckContext context)
        {
            FtpConnectionCheckResult result;

            var lastActivity = context.Connection.LastActivity;

            if (_inactivityTimeout == TimeSpan.MaxValue)
            {
                result = new FtpConnectionCheckResult(true);
            }
            else if (lastActivity < _inactivityTimeout)
            {
                result = new FtpConnectionCheckResult(true);
            }
            else
            {
                _logger.LogInformation(
                    "Connection is idle {ConnectionId} {Stopwatch}",
                    context.Connection.Id,
                    lastActivity);

                result = new FtpConnectionCheckResult(false);
            }

            return result;
        }
    }
}
