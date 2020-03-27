using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Security;

namespace GenerateInputFile
{
    public class NewsPressRelease
    {
        public static DataTable dtResponse = new DataTable();
        public NewsPressRelease()
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
            SecureString secureString = null;
            List list = null;
            ListItemCollection items = null;
            var galleryImage = string.Empty;
            string name = string.Empty, guid = string.Empty, version = string.Empty, fileExtension = string.Empty, fileId = string.Empty, display_order = string.Empty, title = string.Empty,
            documentum_i_chronicle_id = string.Empty, documentum_r_object_id = string.Empty, documentum_content_id = string.Empty;
            try
            {
                using (ClientContext clientContext = new ClientContext(ConfigurationManager.AppSettings.Get(SPOConstants.SPOSiteURL)))
                {
                    secureString = new NetworkCredential("", ConfigurationManager.AppSettings.Get(SPOConstants.SPOPassword)).SecurePassword;
                    clientContext.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings.Get(SPOConstants.SPOUserName), secureString);
                    list = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings.Get(SPOConstants.SPOFolderNewsPressRelease));

                    CamlQuery query = CamlQuery.CreateAllItemsQuery();
                    items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    foreach (var obj in items)
                    {
                        Dictionary<string, object> keyValuePairs = obj.FieldValues;
                        fileId = keyValuePairs.ContainsKey(SPOConstants.Id) ? (keyValuePairs[SPOConstants.Id] != null ? keyValuePairs[SPOConstants.Id].ToString() : "") : "";

                        name = "News_Article_" + fileId + ".xml";
                        //title = keyValuePairs.ContainsKey(SPOConstants.Title) ? (keyValuePairs[SPOConstants.Title] != null ? keyValuePairs[SPOConstants.Title].ToString() : "") : "";
                        guid = keyValuePairs.ContainsKey(SPOConstants.UniqueId) ? (keyValuePairs[SPOConstants.UniqueId] != null ? keyValuePairs[SPOConstants.UniqueId].ToString() : "") : "";
                        version = keyValuePairs.ContainsKey(SPOConstants.UIVersionString) ? (keyValuePairs[SPOConstants.UIVersionString] != null ? keyValuePairs[SPOConstants.UIVersionString].ToString() : "") : "";
                        documentum_i_chronicle_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_i_chronicle_id) ? (keyValuePairs[SPOConstants.Documentum_i_chronicle_id] != null ? keyValuePairs[SPOConstants.Documentum_i_chronicle_id].ToString() : "") : "";
                        documentum_r_object_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_r_object_id) ? (keyValuePairs[SPOConstants.Documentum_r_object_id] != null ? keyValuePairs[SPOConstants.Documentum_r_object_id].ToString() : "") : "";
                        documentum_content_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_content_id) ? (keyValuePairs[SPOConstants.Documentum_content_id] != null ? keyValuePairs[SPOConstants.Documentum_content_id].ToString() : "") : "";
                        FieldLookupValue[] lookup = keyValuePairs.ContainsKey(SPOConstants.GalleryImages) ? (FieldLookupValue[])keyValuePairs[SPOConstants.GalleryImages] : null;
                        display_order = keyValuePairs.ContainsKey(SPOConstants.SortOrder) ? (keyValuePairs[SPOConstants.SortOrder] != null ? keyValuePairs[SPOConstants.SortOrder].ToString() : "0") : "0";

                        dtResponse.Rows.Add(Constants.Ir_article, documentum_r_object_id, documentum_i_chronicle_id, Constants.Articles_r_folder_path, name, FileFormatConstants.XML, Constants.Articles + "/" + name, guid, guid, guid + '@' + guid, Constants.Articles, display_order,"NULL");

                        foreach (var item in lookup)
                        {
                            GetChildItem(clientContext, item.LookupId.ToString(), guid, documentum_i_chronicle_id, documentum_r_object_id, documentum_content_id, display_order, ref dtResponse);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in NewsPressRelease :" + ex.Message);
                dtResponse = null;
            }
            return dtResponse;
        }

        private void GetChildItem(ClientContext clientContext, string Id, string ParentGuid, string i_chronicle_id, string r_object_id, string content_id, string display_order, ref DataTable dtResponse)
        {
            string Name = string.Empty, guid = string.Empty, Version = string.Empty, ListId = string.Empty, title = string.Empty,
                   ThumbnailName = string.Empty; string ParentImageName = string.Empty;
            List list = null;
            ListItemCollection items = null;

            try
            {
                list = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings.Get(SPOConstants.SPOFolderGalleryThumbnail));
                clientContext.Load(list);
                clientContext.ExecuteQuery();

                Folder folder = clientContext.Web.GetFolderByServerRelativeUrl(ConfigurationManager.AppSettings.Get(SPOConstants.SPOSiteURL)
                                                                                       + "/"
                                                                                       + ConfigurationManager.AppSettings.Get(SPOConstants.SPOFolderGalleryThumbnailInternalName) + "/" + Id);
                clientContext.Load(folder);
                clientContext.ExecuteQuery();

                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = @"<View Scope='Recursive'>
                                 <Query>
                                 </Query>
                             </View>";
                camlQuery.FolderServerRelativeUrl = folder.ServerRelativeUrl;

                items = list.GetItems(camlQuery);


                clientContext.Load(items);
                clientContext.ExecuteQuery();

                foreach (var obj in items)
                {
                    Dictionary<string, object> keyValuePairs = obj.FieldValues;
                    if (items.Count == 3)
                    {
                        Dictionary<string, object> keyValuePairsParenName = items[2].FieldValues;
                        ParentImageName = keyValuePairsParenName.ContainsKey(SPOConstants.Name) ? (keyValuePairsParenName[SPOConstants.Name] != null ? keyValuePairsParenName[SPOConstants.Name].ToString() : "") : "";
                    }
                    
                    Name = keyValuePairs.ContainsKey(SPOConstants.Name) ? (keyValuePairs[SPOConstants.Name] != null ? keyValuePairs[SPOConstants.Name].ToString() : "") : "";
                    //title = keyValuePairs.ContainsKey(SPOConstants.Title) ? (keyValuePairs[SPOConstants.Title] != null ? keyValuePairs[SPOConstants.Title].ToString() : "") : "";
                    ListId = keyValuePairs.ContainsKey(SPOConstants.Id) ? (keyValuePairs[SPOConstants.Id] != null ? keyValuePairs[SPOConstants.Id].ToString() : "") : "";       
                    guid = keyValuePairs.ContainsKey(SPOConstants.UniqueId) ? (keyValuePairs[SPOConstants.UniqueId] != null ? keyValuePairs[SPOConstants.UniqueId].ToString() : "") : "";
                    Version = keyValuePairs.ContainsKey(SPOConstants.UIVersionString) ? (keyValuePairs[SPOConstants.UIVersionString] != null ? keyValuePairs[SPOConstants.UIVersionString].ToString() : "") : "";


                    dtResponse.Rows.Add(Constants.Ir_article_image, r_object_id, i_chronicle_id, Constants.Article_images_r_folder_path, ParentImageName, FileFormatConstants.JPEG, Constants.Article_images + "/" + Name, ParentGuid, ParentGuid, guid + '@' + ParentGuid, Constants.Article_images, display_order,"NULL");
                }
            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {

                }
                else
                {
                    Console.WriteLine("Error in NewsPressRelease - GetChildItem :" + ex.Message);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in NewsPressRelease - GetChildItem : " + ex.Message);
                throw ex;
            }
        }
    }
}
