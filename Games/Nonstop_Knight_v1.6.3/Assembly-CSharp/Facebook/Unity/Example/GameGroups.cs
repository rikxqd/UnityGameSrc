﻿namespace Facebook.Unity.Example
{
    using Facebook.Unity;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class GameGroups : MenuBase
    {
        private string gamerGroupCurrentGroup = string.Empty;
        private string gamerGroupDesc = "Test group for testing.";
        private string gamerGroupName = "Test group";
        private string gamerGroupPrivacy = "closed";

        private void CallCreateGroupDialog()
        {
            FB.GameGroupCreate(this.gamerGroupName, this.gamerGroupDesc, this.gamerGroupPrivacy, new FacebookDelegate<IGroupCreateResult>(this.GroupCreateCB));
        }

        private void CallFbGetAllOwnedGroups()
        {
            FB.API(FB.AppId + "/groups", HttpMethod.GET, new FacebookDelegate<IGraphResult>(this.GetAllGroupsCB), (IDictionary<string, string>) null);
        }

        private void CallFbGetUserGroups()
        {
            FB.API("/me/groups?parent=" + FB.AppId, HttpMethod.GET, new FacebookDelegate<IGraphResult>(this.HandleResult), (IDictionary<string, string>) null);
        }

        private void CallFbPostToGamerGroup()
        {
            Dictionary<string, string> formData = new Dictionary<string, string>();
            formData["message"] = "herp derp a post";
            FB.API(this.gamerGroupCurrentGroup + "/feed", HttpMethod.POST, new FacebookDelegate<IGraphResult>(this.HandleResult), formData);
        }

        private void CallJoinGroupDialog()
        {
            FB.GameGroupJoin(this.gamerGroupCurrentGroup, new FacebookDelegate<IGroupJoinResult>(this.HandleResult));
        }

        private void GetAllGroupsCB(IGraphResult result)
        {
            if (!string.IsNullOrEmpty(result.RawResult))
            {
                base.LastResponse = result.RawResult;
                IDictionary<string, object> resultDictionary = result.ResultDictionary;
                if (resultDictionary.ContainsKey("data"))
                {
                    List<object> list = (List<object>) resultDictionary["data"];
                    if (list.Count > 0)
                    {
                        Dictionary<string, object> dictionary2 = (Dictionary<string, object>) list[0];
                        this.gamerGroupCurrentGroup = (string) dictionary2["id"];
                    }
                }
            }
            if (!string.IsNullOrEmpty(result.Error))
            {
                base.LastResponse = result.Error;
            }
        }

        protected override void GetGui()
        {
            if (base.Button("Game Group Create - Closed"))
            {
                FB.GameGroupCreate("Test game group", "Test description", "CLOSED", new FacebookDelegate<IGroupCreateResult>(this.HandleResult));
            }
            if (base.Button("Game Group Create - Open"))
            {
                FB.GameGroupCreate("Test game group", "Test description", "OPEN", new FacebookDelegate<IGroupCreateResult>(this.HandleResult));
            }
            base.LabelAndTextField("Group Name", ref this.gamerGroupName);
            base.LabelAndTextField("Group Description", ref this.gamerGroupDesc);
            base.LabelAndTextField("Group Privacy", ref this.gamerGroupPrivacy);
            if (base.Button("Call Create Group Dialog"))
            {
                this.CallCreateGroupDialog();
            }
            base.LabelAndTextField("Group To Join", ref this.gamerGroupCurrentGroup);
            bool enabled = GUI.enabled;
            GUI.enabled = enabled && !string.IsNullOrEmpty(this.gamerGroupCurrentGroup);
            if (base.Button("Call Join Group Dialog"))
            {
                this.CallJoinGroupDialog();
            }
            GUI.enabled = enabled && FB.IsLoggedIn;
            if (base.Button("Get All App Managed Groups"))
            {
                this.CallFbGetAllOwnedGroups();
            }
            if (base.Button("Get Gamer Groups Logged in User Belongs to"))
            {
                this.CallFbGetUserGroups();
            }
            if (base.Button("Make Group Post As User"))
            {
                this.CallFbPostToGamerGroup();
            }
            GUI.enabled = enabled;
        }

        private void GroupCreateCB(IGroupCreateResult result)
        {
            base.HandleResult(result);
            if (result.GroupId != null)
            {
                this.gamerGroupCurrentGroup = result.GroupId;
            }
        }
    }
}

