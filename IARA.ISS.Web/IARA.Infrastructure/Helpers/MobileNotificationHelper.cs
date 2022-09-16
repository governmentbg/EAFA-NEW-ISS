using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;

namespace IARA.Infrastructure.Services
{
    internal static class MobileNotificationHelper
    {
        private const int MOBILE_NOTIFICATION_TIMEOUT_SECONDS = 30;

        public static IEnumerable<string> SendMobileNotifications(this FirebaseMessaging firebaseMessaging,
                                                                  ICollection<Message> messagesList,
                                                                  out List<Exception> exceptions,
                                                                  out List<string> invalidTokens)
        {
            exceptions = new List<Exception>();
            invalidTokens = new List<string>();

            if (messagesList != null)
            {
                foreach (var message in messagesList)
                {
                    try
                    {
                        firebaseMessaging.SendAsync(message).Wait(TimeSpan.FromSeconds(MOBILE_NOTIFICATION_TIMEOUT_SECONDS));
                    }
                    catch (FirebaseMessagingException ex)
                    {
                        ex.LogInvalidToken(message.Token, invalidTokens);
                        exceptions.Add(ex);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);

                        while (ex != null && ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                            if (ex is FirebaseMessagingException firebaseException)
                            {
                                firebaseException.LogInvalidToken(message.Token, invalidTokens);
                                break;
                            }
                        }

                    }
                }
            }

            return messagesList.Where(x => !string.IsNullOrEmpty(x.Token)).Select(x => x.Token).ToList().Except(invalidTokens);
        }


        private static void LogInvalidToken(this FirebaseMessagingException firebaseException, string token, List<string> invalidTokens)
        {
            if (!string.IsNullOrEmpty(token)
                                         && (firebaseException.ErrorCode == FirebaseAdmin.ErrorCode.NotFound
                                         || firebaseException.ErrorCode == FirebaseAdmin.ErrorCode.Unauthenticated))
            {
                invalidTokens.Add(token);
            }
        }
    }
}
