using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Security;

namespace GenerateInputFile
{
    public class Events
    {
        public static DataTable dtResponse = new DataTable();

        public Events()
        {

            dtResponse.Columns.Add(Constants.R_object_type);
            dtResponse.Columns.Add(Constants.Documentum_r_object_id);
            dtResponse.Columns.Add(Constants.Documentum_i_chronicle_id);
            dtResponse.Columns.Add(Constants.Documentum_r_folder_path);
            //dtResponse.Columns.Add(Constants.Title);
            dtResponse.Columns.Add(Constants.FileName);
            dtResponse.Columns.Add(Constants.I_full_format);
            dtResponse.Columns.Add(Constants.A_webc_url);
            dtResponse.Columns.Add(Constants.R_object_id);
            dtResponse.Columns.Add(Constants.I_chronicle_id);
            dtResponse.Columns.Add(Constants.Content_id);
            dtResponse.Columns.Add(Constants.R_folder_path);
            dtResponse.Columns.Add(Constants.Display_Order);
            dtResponse.Columns.Add(Constants.FISCAL_QTR);
        }

        public DataTable GetListFileId()
        {
            string Name = string.Empty, guid = string.Empty, Version = string.Empty, display_order = string.Empty, title = string.Empty, fileId = string.Empty,
                documentum_i_chronicle_id = string.Empty, documentum_r_object_id = string.Empty, documentum_content_id = string.Empty;
            SecureString secureString = null;
            List list = null;
            ListItemCollection items = null;
            try
            {
                using (ClientContext clientContext = new ClientContext(ConfigurationManager.AppSettings.Get(SPOConstants.SPOSiteURL)))
                {
                    secureString = new NetworkCredential("", ConfigurationManager.AppSettings.Get(SPOConstants.SPOPassword)).SecurePassword;
                    clientContext.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings.Get(SPOConstants.SPOUserName), secureString);
                    list = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings.Get(SPOConstants.SPOFolderEvents));

                    CamlQuery query = CamlQuery.CreateAllItemsQuery();
                    items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    foreach (var obj in items)
                    {
                        Dictionary<string, object> keyValuePairs = obj.FieldValues;

                        fileId = keyValuePairs.ContainsKey(SPOConstants.Id) ? (keyValuePairs[SPOConstants.Id] != null ? keyValuePairs[SPOConstants.Id].ToString() : "") : "";
                        //title = keyValuePairs.ContainsKey(SPOConstants.Title) ? (keyValuePairs[SPOConstants.Title] != null ? keyValuePairs[SPOConstants.Title].ToString() : "") : "";
                        Name = "Event_" + fileId;
                        guid = keyValuePairs.ContainsKey(SPOConstants.UniqueId) ? (keyValuePairs[SPOConstants.UniqueId] != null ? keyValuePairs[SPOConstants.UniqueId].ToString() : "") : "";
                        Version = keyValuePairs.ContainsKey(SPOConstants.UIVersionString) ? (keyValuePairs[SPOConstants.UIVersionString] != null ? keyValuePairs[SPOConstants.UIVersionString].ToString() : "") : "";
                        documentum_i_chronicle_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_i_chronicle_id) ? (keyValuePairs[SPOConstants.Documentum_i_chronicle_id] != null ? keyValuePairs[SPOConstants.Documentum_i_chronicle_id].ToString() : "") : "";
                        documentum_r_object_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_r_object_id) ? (keyValuePairs[SPOConstants.Documentum_r_object_id] != null ? keyValuePairs[SPOConstants.Documentum_r_object_id].ToString() : "") : "";
                        documentum_content_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_content_id) ? (keyValuePairs[SPOConstants.Documentum_content_id] != null ? keyValuePairs[SPOConstants.Documentum_content_id].ToString() : "") : "";
                        display_order = keyValuePairs.ContainsKey(SPOConstants.SortOrder) ? (keyValuePairs[SPOConstants.SortOrder] != null ? keyValuePairs[SPOConstants.SortOrder].ToString() : "0") : "0";

                        dtResponse.Rows.Add(Constants.Ir_event, documentum_r_object_id, documentum_i_chronicle_id, Constants.Events_r_folder_path, Name, FileFormatConstants.Contentless, Constants.Events + "/" + Name, guid, guid, guid + '@' + guid, Constants.Events, display_order,"NULL");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Events :" + ex.Message);
                dtResponse = null;
            }
            return dtResponse;
        }
    }
}
