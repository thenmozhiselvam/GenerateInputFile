﻿using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Security;

namespace GenerateInputFile
{
    public class TenKTenQ
    {
        public static DataTable dtResponse = new DataTable();

        public TenKTenQ()
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
            string name = string.Empty, guid = string.Empty, version = string.Empty, fileExtension = string.Empty, display_order = string.Empty, title = string.Empty,
                documentum_i_chronicle_id = string.Empty, documentum_r_object_id = string.Empty, documentum_content_id = string.Empty; string fiscal_qtr = string.Empty;
            try
            {
                using (ClientContext clientContext = new ClientContext(ConfigurationManager.AppSettings.Get(SPOConstants.SPOSiteURL)))
                {
                    secureString = new NetworkCredential("", ConfigurationManager.AppSettings.Get(SPOConstants.SPOPassword)).SecurePassword;
                    clientContext.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings.Get(SPOConstants.SPOUserName), secureString);
                    list = clientContext.Web.Lists.GetByTitle(ConfigurationManager.AppSettings.Get(SPOConstants.SPOFolder10K10Q));

                    CamlQuery query = CamlQuery.CreateAllItemsQuery();
                    items = list.GetItems(query);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    foreach (var obj in items)
                    {
                        Dictionary<string, object> keyValuePairs = obj.FieldValues;

                        //title = keyValuePairs.ContainsKey(SPOConstants.Title) ? (keyValuePairs[SPOConstants.Title] != null ? keyValuePairs[SPOConstants.Title].ToString() : "") : "";
                        name = keyValuePairs.ContainsKey(SPOConstants.Name) ? (keyValuePairs[SPOConstants.Name] != null ? keyValuePairs[SPOConstants.Name].ToString() : "") : "";
                        guid = keyValuePairs.ContainsKey(SPOConstants.UniqueId) ? (keyValuePairs[SPOConstants.UniqueId] != null ? keyValuePairs[SPOConstants.UniqueId].ToString() : "") : "";
                        version = keyValuePairs.ContainsKey(SPOConstants.UIVersionString) ? (keyValuePairs[SPOConstants.UIVersionString] != null ? keyValuePairs[SPOConstants.UIVersionString].ToString() : "") : "";
                        documentum_i_chronicle_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_i_chronicle_id) ? (keyValuePairs[SPOConstants.Documentum_i_chronicle_id] != null ? keyValuePairs[SPOConstants.Documentum_i_chronicle_id].ToString() : "") : "";
                        documentum_r_object_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_r_object_id) ? (keyValuePairs[SPOConstants.Documentum_r_object_id] != null ? keyValuePairs[SPOConstants.Documentum_r_object_id].ToString() : "") : "";
                        documentum_content_id = keyValuePairs.ContainsKey(SPOConstants.Documentum_content_id) ? (keyValuePairs[SPOConstants.Documentum_content_id] != null ? keyValuePairs[SPOConstants.Documentum_content_id].ToString() : "") : "";
                        display_order = keyValuePairs.ContainsKey(SPOConstants.SortOrder) ? (keyValuePairs[SPOConstants.SortOrder] != null ? keyValuePairs[SPOConstants.SortOrder].ToString() : "0") : "0";
                        fiscal_qtr = keyValuePairs.ContainsKey(SPOConstants.FiscalPeriod) ? (keyValuePairs[SPOConstants.FiscalPeriod] != null ? keyValuePairs[SPOConstants.FiscalPeriod].ToString() : "") : "";
                        fileExtension = System.IO.Path.GetExtension(name).Replace('.', ' ').Trim();

                        dtResponse.Rows.Add(Constants.Ir_qtrly_report, documentum_r_object_id, documentum_i_chronicle_id, Constants.Quarterly_reports_r_folder_path, name, fileExtension, Constants.Quarterly_reports + "/" + name, guid, guid, guid + '@' + guid, Constants.Quarterly_reports, display_order, fiscal_qtr);

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :" + ex.Message);
                dtResponse = null;
            }
            return dtResponse;
        }
    }
}
