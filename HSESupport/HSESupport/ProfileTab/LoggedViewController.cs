using CoreGraphics;
using FFImageLoading;
using Foundation;
using System;
using System.IO;
using System.Threading;
using UIKit;
using UserNotifications;

namespace HSESupport
{
    public partial class LoggedViewController : UIViewController
    {
        UILabel UserInitials;
        static UIImage ProfileImage;
        UIStoryboard storyboard;
        public LoggedViewController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            storyboard = UIStoryboard.FromName("Main", null);
            UserInitials = new UILabel()
            {
                TextAlignment = UITextAlignment.Center,
                Frame = new CGRect(0, 0, UserPicture.Frame.Width, UserPicture.Frame.Height),
                TextColor = UIColor.White,
                Font = UIFont.FromDescriptor(UIFont.PreferredTitle1.FontDescriptor, 48f)
            };
            UserPicture.AddSubview(UserInitials);
            LogOutButton.TouchUpInside += async (x, y) =>
            {
                if (File.Exists(Constants.Images + Profile.Instance.UserId + ".jpg"))
                {
                    File.Delete(Constants.Images + Profile.Instance.UserId + ".jpg");
                }
                await RemoteService.LogOut();
                ProfileImage = null;
                ((ProfileViewController)(ParentViewController.ParentViewController)).PresentContainerView(0);
                RemoteService.NeededProfilePageNum = 0;
                UIApplication.SharedApplication.UnregisterForRemoteNotifications();
            };

            ChoosePicture.UserInteractionEnabled = true;
            UILongPressGestureRecognizer longp = new UILongPressGestureRecognizer(LongPress);
            ChoosePicture.AddGestureRecognizer(longp);
        }
        public void LongPress()
        {
            Console.WriteLine("Long press happened");
            var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create("Delete User Picture", UIAlertActionStyle.Destructive, async (action) =>
            {
                ProfileImage = null;
                if (File.Exists(Constants.Images + Profile.Instance.UserId + ".jpg"))
                {
                    File.Delete(Constants.Images + Profile.Instance.UserId + ".jpg");
                }
                await RemoteService.DeleteUserPicture(Profile.Instance);
                ViewWillAppear(false);
            }));
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (Profile.Instance != null)
            {
                FullNameLabel.Text = Profile.Instance.Name;
                StatusLabel.Text = Profile.Instance.Status;
                EmailLabel.Text = Profile.Instance.Email;
                string[] name = Profile.Instance.Name.Split(' ');
                if (Profile.Instance.Status == "Student")
                {
                    CreateAlertForAll.Hidden = true;
                }
                else
                {
                    CreateAlertForAll.Hidden = false;
                }
                if (File.Exists(Constants.Images + Profile.Instance.UserId + ".jpg"))
                {
                    UserInitials.Text = string.Empty;
                    ProfileImage = UIImage.FromFile(Constants.Images + Profile.Instance.UserId + ".jpg");
                    UserPicture.Image = ProfileImage;
                }
                else if (Profile.Instance.HasPicture == 1)
                {
                    new Thread(async () =>
                    {
                        await RemoteService.GetUserPicture(Profile.Instance, Constants.Images);
                        InvokeOnMainThread(() =>
                        {
                            if (File.Exists(Constants.Images + Profile.Instance.UserId + ".jpg"))
                            {
                                UserInitials.Text = string.Empty;
                                ProfileImage = UIImage.FromFile(Constants.Images + Profile.Instance.UserId + ".jpg");
                                UserPicture.Image = ProfileImage;
                            }
                        });
                    }).Start();
                }
                else
                {
                    UserInitials.Text = char.ToUpper(name[0][0]).ToString() + char.ToUpper(name[1][0]).ToString();
                    UserPicture.Image = UIImage.FromFile("Images/PicBG@3x.png");
                }
            }
            else
            {
                UserPicture = null;
            }
        }

        UIImagePickerController picker;
        partial void ChoosePicture_Pressed(UIButton sender)
        {
            picker = new UIImagePickerController();
            picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            picker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            picker.FinishedPickingMedia += Finished;
            picker.Canceled += Canceled;
            PresentViewController(picker, animated: true, completionHandler: null);
        }

        public void Finished(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    isImage = true;
                    break;
                case "public.video":
                    break;
            }
            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
            if (referenceURL != null) Console.WriteLine("Url:" + referenceURL.ToString());
            if (isImage)
            {
                UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
                if (originalImage != null)
                {
                    try
                    {
                        if (File.Exists(Constants.Images + Profile.Instance.UserId + ".jpg"))
                        {
                            Console.WriteLine("Found a picture in Images folder for current user");
                            File.Delete(Constants.Images + Profile.Instance.UserId + ".jpg");
                        }
                        originalImage.AsJPEG().Save(Constants.Images + Profile.Instance.UserId + "orig.jpg", true);
                        Console.WriteLine("OriginalImage height: " + originalImage.CGImage.Height);

                        if (!File.Exists(Constants.Images + Profile.Instance.UserId + "orig.jpg"))
                        {
                            throw new Exception();
                        }
                        new Thread(new ThreadStart(async () =>
                        {
                            UIImage lowQualityImage = await ImageService.Instance.LoadFile(Constants.Images + Profile.Instance.UserId + "orig.jpg")
                                            .DownSample(height: 100)
                                            .AsUIImageAsync();
                            if (File.Exists(Constants.Images + Profile.Instance.UserId + ".jpg"))
                            {
                                File.Delete(Constants.Images + Profile.Instance.UserId + ".jpg");
                            }
                            Console.WriteLine("lowQualityImage height: " + lowQualityImage.CGImage.Height);
                            lowQualityImage.AsJPEG().Save(Constants.Images + Profile.Instance.UserId + ".jpg", false);

                            InvokeOnMainThread(() =>
                            {
                                if (UserInitials != null)
                                {
                                    UserInitials.Text = string.Empty;
                                }
                                UserPicture.Image = lowQualityImage;
                            });

                            await RemoteService.SetUserPicture(Profile.Instance);
                        })).Start();
                    } catch (Exception) { }
                }
            }
            else
            {
                NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
                if (mediaURL != null)
                {
                    Console.WriteLine(mediaURL.ToString());
                }
            }
            picker.DismissModalViewController(true);
        }

        void Canceled(object sender, EventArgs e)
        {
            picker.DismissModalViewController(true);
        }

        partial void CreateAlertForAll_Pressed(UIButton sender)
        {
            if (storyboard != null)
            {
                OneMoreNavigationController viewController = storyboard.InstantiateViewController("OneMoreNavigationController") as OneMoreNavigationController;
                if (viewController != null)
                {
                    viewController.Title = "Create Alert";
                    PresentModalViewController(viewController, true);
                }
            }
        }
    }
}