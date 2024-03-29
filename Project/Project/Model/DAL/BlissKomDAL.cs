﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Project.PageModel;

namespace Project.Model.DAL
{
    public class BlissKomDAL : DALBase
    {

        #region navigation/presentation

        /// <summary>
        /// Hämta den information från tre tabeller som har med navigationssidans Css-inställningar att göra.
        /// </summary>
        /// <param name="categoryId">kategori-id:t</param>
        /// <param name="pageNumber">sidnummer</param>
        /// <returns></returns>
        public string SelectPageInfo(int categoryId, int pageNumber)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    string cssTemplate = String.Empty;
                    //int numberOfPositions = 0;

                    var cmd = new SqlCommand("appSchema.usp_SelectPageInfo", conn);
                    cmd.Parameters.Add("@CatId", SqlDbType.SmallInt, 2);
                    cmd.Parameters.Add("@CatPageNum", SqlDbType.TinyInt, 1);
                    cmd.Parameters["@CatId"].Value = (Int16)categoryId;
                    cmd.Parameters["@CatPageNum"].Value = (byte)pageNumber;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var cssTemplateNameIndex = reader.GetOrdinal("CssTemplateName");
                        //var numberOfPositionsIndex = reader.GetOrdinal("NumberOfPositions");

                        if (reader.Read())
                        {
                            cssTemplate = reader.GetString(cssTemplateNameIndex);
                            //numberOfPositions = (int)reader.GetByte(numberOfPositionsIndex);
                        }
                    }

                    return cssTemplate;
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }


        /// <summary>
        /// Hämtar all den information från nio olika tabeller som behövs för att presentera en navigationssida.
        /// </summary>
        /// <param name="categoryId">kategori-id:t</param>
        /// <param name="pageNumber">sidnumret</param>
        /// <returns>Samling med PageItem:s</returns>
        public IEnumerable<PageItem> SelectPageItemsOfPage(int categoryId, int pageNumber)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var pageItems = new List<PageItem>();

                    var cmd = new SqlCommand("appSchema.usp_SelectFullPage", conn);
                    cmd.Parameters.Add("@CatId", SqlDbType.SmallInt, 2);
                    //cmd.Parameters.Add("@CatPageNum", SqlDbType.TinyInt, 1);
                    cmd.Parameters["@CatId"].Value = (Int16)categoryId;
                    //cmd.Parameters["@CatPageNum"].Value = (byte)pageNumber;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {

                        var backGroundColorIndex = reader.GetOrdinal("BackGroundColor");
                        var pageWTypeIndex = reader.GetOrdinal("PageWType");
                        var meaningWordIndex = reader.GetOrdinal("MeaningWord");
                        var meaningCommentIndex = reader.GetOrdinal("MeaningComment");
                        var meaningIdIndex = reader.GetOrdinal("MeaningId");
                        var pageItemTypeIndex = reader.GetOrdinal("PageItemType");
                        var positionIndex = reader.GetOrdinal("Position");
                        var pageImageTypeIndex = reader.GetOrdinal("PageImageType");
                        var imageCommentIndex = reader.GetOrdinal("ImageComment");
                        var imageFileNameIndex = reader.GetOrdinal("ImageFileName");
                        var catNameIndex = reader.GetOrdinal("CatName");
                        var catRefIdIndex = reader.GetOrdinal("CatRefId");
                        var pageItemIdIndex = reader.GetOrdinal("ItemId");
                        //var cssTemplateNameIndex = reader.GetOrdinal("cssTemplateName");

                        while (reader.Read())
                        {
                            var pageItemType = reader.GetString(pageItemTypeIndex);
                            var catRefId = reader.IsDBNull(catRefIdIndex) ?
                                null : (int?)reader.GetInt16(catRefIdIndex);
                            PageItem pageItem;

                            if (pageItemType.Equals("Parent"))
                            {
                                if (catRefId == null)
                                {
                                    pageItem = new PageParentWordItem()
                                    {
                                    };
                                }
                                else
                                {
                                    pageItem = new PageParentCategoryItem()
                                    {
                                        LinkToCategoryId = (int)catRefId
                                    };
                                }
                            }
                            else if (pageItemType.Equals("LeftChild"))
                            {
                                pageItem = new PageChildWordItem()
                                {
                                    PageItemType = PageItemType.ChildLeftWordItem
                                };
                            }
                            else if (pageItemType.Equals("RightChild"))
                            {
                                pageItem = new PageChildWordItem()
                                {
                                    PageItemType = PageItemType.ChildRightWordItem
                                };
                            }
                            else
                            {
                                //måste tillhöra en typ, skippa annars aktuellt pageitem
                                continue;
                            }

                            pageItem.BackGroundRGBColor = reader.GetString(backGroundColorIndex);
                            pageItem.MeaningWord = reader.GetString(meaningWordIndex);
                            pageItem.MeaningComment = reader.GetString(meaningCommentIndex);
                            pageItem.MeaningId = (int)reader.GetInt16(meaningIdIndex);
                            pageItem.Position = (int)reader.GetInt16(positionIndex);
                            switch (reader.GetString(pageImageTypeIndex))
                            {
                                case "Blissymbol":
                                    pageItem.PageImageType = PageImageType.Blissymbol;
                                    break;
                                case "SignLanguage":
                                    pageItem.PageImageType = PageImageType.SignLanguage;
                                    break;
                                case "Photo":
                                    pageItem.PageImageType = PageImageType.Photo;
                                    break;
                            }
                            pageItem.ImageComment = reader.GetString(imageCommentIndex);
                            pageItem.ImageFileName = reader.GetString(imageFileNameIndex);
                            //pageItem.CssTemplateName = reader.GetString(cssTemplateNameIndex);
                            pageItem.PageItemId = (int)reader.GetInt16(pageItemIdIndex);

                            pageItems.Add(pageItem);
                        }
                    }
                    return pageItems;
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        #endregion

        #region insert/update/delete

        /// <summary>
        /// Lägger till en post i Meaning-tabellen.
        /// </summary>
        /// <param name="meaning">Meaning-objektet</param>
        public void InsertMeaning(Meaning meaning)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_InsertMeaning", conn);
                    cmd.Parameters.Add("@WTypeId", SqlDbType.TinyInt, 1);
                    cmd.Parameters["@WTypeId"].Value = meaning.WTypeId;
                    cmd.Parameters.Add("@Word", SqlDbType.VarChar, 30);
                    cmd.Parameters["@Word"].Value = meaning.Word;
                    cmd.Parameters.Add("@Comment", SqlDbType.VarChar, 100);
                    cmd.Parameters["@Comment"].Value = meaning.Comment;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att lägga till informationen i databasen.");
                }
            }
        }

        /// <summary>
        /// Uppdaterar en post i Meaning-tabellen.
        /// </summary>
        /// <param name="meaning">Meaning-objektet</param>
        public void UpdateMeaning(Meaning meaning)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_UpdateMeaning", conn);
                    cmd.Parameters.Add("@MeaningId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@MeaningId"].Value = meaning.MeaningId;
                    cmd.Parameters.Add("@WTypeId", SqlDbType.TinyInt, 1);
                    cmd.Parameters["@WTypeId"].Value = meaning.WTypeId;
                    cmd.Parameters.Add("@Word", SqlDbType.VarChar, 30);
                    cmd.Parameters["@Word"].Value = meaning.Word;
                    cmd.Parameters.Add("@Comment", SqlDbType.VarChar, 100);
                    cmd.Parameters["@Comment"].Value = meaning.Comment;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att uppdatera informationen i databasen.");
                }
            }
        }

        /// <summary>
        /// Raderar en post från Meaning-tabellen.
        /// </summary>
        /// <param name="meaningId">Postens MeaningId</param>
        public void DeleteMeaning(Int16 meaningId)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_DeleteMeaningWithItems", conn);
                    cmd.Parameters.Add("@MeaningId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@MeaningId"].Value = meaningId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att radera från databasen.");
                }
            }
        }

        /// <summary>
        /// Lägger till en post i Item-tabellen.
        /// </summary>
        /// <param name="item">Item-objektet</param>
        public void InsertItem(Item item)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_InsertItem", conn);
                    cmd.Parameters.Add("@MeaningId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@MeaningId"].Value = item.MeaningId;
                    cmd.Parameters.Add("@ImageId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@ImageId"].Value = item.ImageId;
                    cmd.Parameters.Add("@CatId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@CatId"].Value = item.CatId;
                    cmd.Parameters.Add("@CatRefId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@CatRefId"].Value = item.CatRefId;
                    cmd.Parameters.Add("@PosId", SqlDbType.TinyInt, 1);
                    cmd.Parameters["@PosId"].Value = item.PosId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att lägga till informationen i databasen.");
                }
            }
        }

        /// <summary>
        /// Uppdaterar en post i Item-tabellen.
        /// </summary>
        /// <param name="item">Item-objektet</param>
        public void UpdateItem(Item item)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_UpdateItem", conn);
                    cmd.Parameters.Add("@ItemId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@ItemId"].Value = item.ItemId;
                    cmd.Parameters.Add("@MeaningId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@MeaningId"].Value = item.MeaningId;
                    cmd.Parameters.Add("@ImageId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@ImageId"].Value = item.ImageId;
                    cmd.Parameters.Add("@CatId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@CatId"].Value = item.CatId;
                    cmd.Parameters.Add("@CatRefId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@CatRefId"].Value = item.CatRefId;
                    cmd.Parameters.Add("@PosId", SqlDbType.TinyInt, 1);
                    cmd.Parameters["@PosId"].Value = item.PosId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att uppdatera informationen i databasen.");
                }
            }
        }

        /// <summary>
        /// Raderar en post från Item-tabellen.
        /// </summary>
        /// <param name="itemId">Postens ItemId</param>
        public void DeleteItem(Int16 itemId)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_DeleteItem", conn);
                    cmd.Parameters.Add("@ItemId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@ItemId"].Value = itemId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att radera från databasen.");
                }
            }
        }

        #endregion

        #region select *

        /// <summary>
        /// Hämtar alla poster från Meaning-tabellen.
        /// </summary>
        /// <returns>Samling med Meaning-objekt.</returns>
        public IEnumerable<Meaning> SelectAllMeanings()
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var meanings = new List<Meaning>();

                    var cmd = new SqlCommand("appSchema.usp_SelectAllMeanings", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var meaningIdIndex = reader.GetOrdinal("MeaningId");
                        var wTypeIdIndex = reader.GetOrdinal("WTypeId");
                        var wordIndex = reader.GetOrdinal("Word");
                        var commentIndex = reader.GetOrdinal("Comment");

                        while (reader.Read())
                        {
                            meanings.Add(new Meaning
                            {
                                MeaningId = reader.GetInt16(meaningIdIndex),
                                WTypeId = reader.GetByte(wTypeIdIndex),
                                Word = reader.GetString(wordIndex),
                                Comment = reader.GetString(commentIndex)
                            });
                        }
                    }

                    return meanings;

                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämtar alla poster från Item-tabellen.
        /// </summary>
        /// <returns>Samling med Item-objekt.</returns>
        public IEnumerable<Item> SelectAllItems()
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var items = new List<Item>();

                    var cmd = new SqlCommand("appSchema.usp_SelectAllItems", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {

                        var itemIdIndex = reader.GetOrdinal("ItemId");
                        var meaningIdIndex = reader.GetOrdinal("MeaningId");
                        var imageIdIndex = reader.GetOrdinal("ImageId");
                        var catRefIdIndex = reader.GetOrdinal("CatRefId");
                        var catIdIndex = reader.GetOrdinal("CatId");
                        var posIdIndex = reader.GetOrdinal("PosId");

                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                ItemId = reader.GetInt16(itemIdIndex),
                                MeaningId = reader.GetInt16(meaningIdIndex),
                                ImageId = reader.GetInt16(imageIdIndex),
                                CatRefId = reader.GetInt16(catRefIdIndex),
                                CatId = reader.GetInt16(catIdIndex),
                                PosId = reader.GetByte(posIdIndex)
                            });
                        }
                    }

                    return items;

                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämtar alla poster från Category-tabellen.
        /// </summary>
        /// <returns>Associativ vektor med CatId (key) och CatName (value)</returns>
        public Dictionary<int, string> SelectAllCategories()
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_SelectAllCategories", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    var categories = new Dictionary<int, string>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var catIdIndex = reader.GetOrdinal("CatId");
                        var catNameIndex = reader.GetOrdinal("CatName");

                        while (reader.Read())
                        {
                            categories.Add(
                                (int)reader.GetInt16(catIdIndex),
                                reader.GetString(catNameIndex)
                            );
                        }
                        return categories;
                    }
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämtar ett Meaning-objekt med inskickat MeaningId.
        /// </summary>
        /// <param name="meaningId">MeaningId</param>
        /// <returns>Meaning-objektet.</returns>
        public Meaning SelectMeaning(Int16 meaningId)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_SelectMeaning", conn);
                    cmd.Parameters.Add("@MeaningId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@MeaningId"].Value = meaningId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var meaningIdIndex = reader.GetOrdinal("MeaningId");
                        var wTypeIdIndex = reader.GetOrdinal("WTypeId");
                        var wordIndex = reader.GetOrdinal("Word");
                        var commentIndex = reader.GetOrdinal("Comment");

                        if (reader.Read())
                        {
                            return new Meaning()
                            {
                                MeaningId = reader.GetInt16(meaningIdIndex),
                                WTypeId = reader.GetByte(wTypeIdIndex),
                                Word = reader.GetString(wordIndex),
                                Comment = reader.GetString(commentIndex)
                            };
                        }
                    }
                    return null;
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        #endregion

        #region select special

        /// <summary>
        /// Hämtar PosId och en strängrepresentation av denna bestående av information 
        /// från Position- och PositionType-tabellerna.
        /// </summary>
        /// <returns>Associativ vektor med PosId (key) och en textrepresentation av denna (value).</returns>
        public Dictionary<int, string> SelectAllPositions()
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_SelectAllPositionsSpecial", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    var positions = new Dictionary<int, string>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var posIdIndex = reader.GetOrdinal("PosId");
                        var positionIndex = reader.GetOrdinal("Position");

                        while (reader.Read())
                        {
                            positions.Add(
                                (int)reader.GetByte(posIdIndex),
                                reader.GetString(positionIndex)
                            );
                        }
                        return positions;
                    }
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämtar ImageId och FileName från alla poster i Image-tabellen.
        /// </summary>
        /// <returns>Associativ vektor med ImageId (key) och FileName (value)</returns>
        public Dictionary<int, string> SelectAllFileNames()
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_SelectAllImagesSpecial", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    var fileNames = new Dictionary<int, string>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var imageIdIndex = reader.GetOrdinal("ImageId");
                        var fileNameIndex = reader.GetOrdinal("FileName");

                        while (reader.Read())
                        {
                            fileNames.Add(
                                (int)reader.GetInt16(imageIdIndex),
                                reader.GetString(fileNameIndex)
                            );
                        }
                        return fileNames;
                    }
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämtar alla poster från WordType-tabellen och därtill hörande ColorRGBCode från Color-tabellen
        /// och skapar anpassade PageWordType-objekt av den informationen.
        /// </summary>
        /// <returns>Samling med PageWordType-objekt</returns>
        public IEnumerable<PageWordType> SelectAllPageWordTypes()
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var pageWordTypes = new List<PageWordType>();

                    var cmd = new SqlCommand("appSchema.usp_SelectAllWordTypesWithColorCode", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var wTypeIdIndex = reader.GetOrdinal("WTypeId");
                        var colorIdIndex = reader.GetOrdinal("ColorId");
                        var wTypeIndex = reader.GetOrdinal("WType");
                        var colorRGBCodeIndex = reader.GetOrdinal("ColorRGBCode");

                        while (reader.Read())
                        {
                            pageWordTypes.Add(new PageWordType
                            {
                                WTypeId = (int)reader.GetByte(wTypeIdIndex),
                                ColorId = (int)reader.GetByte(colorIdIndex),
                                WType = reader.GetString(wTypeIndex),
                                ColorRGBCode = reader.GetString(colorRGBCodeIndex)
                            });
                        }
                    }

                    return pageWordTypes;

                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämta CatId och ev. CatRefId som hör till inskickat MeaningId.
        /// </summary>
        /// <param name="meaningId">MeaningId</param>
        /// <returns>CatId (key), CatRefId (value). Om CatRefId saknas ges value värdet null.</returns>
        public KeyValuePair<int, int?> SelectCatInfoOfMeaning(Int16 meaningId)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_SelectCatInfoOfMeaning", conn);
                    cmd.Parameters.Add("@MeaningId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@MeaningId"].Value = meaningId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    int catId = 0;
                    int? catRefId = null;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var catIdIndex = reader.GetOrdinal("CatId");
                        var catRefIdIndex = reader.GetOrdinal("CatRefId");

                        while (reader.Read())
                        {
                            catId = (int)reader.GetInt16(catIdIndex);
                            if (catRefId == null)
                            {
                                catRefId = reader.IsDBNull(catRefIdIndex) ?
                                        null : (int?)reader.GetInt16(catRefIdIndex);
                            }
                        }
                        //Key = CatId, samma för alla sökresultat
                        //Value = CatRefId, finns för ett eller noll av sökresultaten
                        if (catId > 0)
                        {
                            return new KeyValuePair<int, int?>(
                                        catId,
                                        catRefId);
                        }
                        else
                        {
                            return new KeyValuePair<int, int?>();
                        }
                    }
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämta alla ItemId och FileName som hör till inskickat MeaningId.
        /// </summary>
        /// <param name="meaningId">MeaningId</param>
        /// <returns>Associativ vektor med ItemId (key) och FileName (value).</returns>
        public Dictionary<int, string> SelectPageItemFileNames(Int16 meaningId)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_SelectFileNamesOfPageItemsByMeaningId", conn);
                    cmd.Parameters.Add("@MeaningId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@MeaningId"].Value = meaningId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    var fileNames = new Dictionary<int, string>();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var itemIdIndex = reader.GetOrdinal("ItemId");
                        var fileNameIndex = reader.GetOrdinal("FileName");

                        while (reader.Read())
                        {
                            fileNames.Add(
                                (int)reader.GetInt16(itemIdIndex),
                                reader.GetString(fileNameIndex)
                            );
                        }
                        return fileNames;
                    }
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        /// <summary>
        /// Hämta PosId som hör till inskickad ItemId.
        /// </summary>
        /// <param name="itemId">ItemId</param>
        /// <returns>PosId</returns>
        public int SelectPositionIdOfItem(Int16 itemId)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    var cmd = new SqlCommand("appSchema.usp_SelectPosIdByItemId", conn);
                    cmd.Parameters.Add("@ItemId", SqlDbType.SmallInt, 2);
                    cmd.Parameters["@ItemId"].Value = itemId;
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var posIdIndex = reader.GetOrdinal("PosId");

                        if (reader.Read())
                        {
                            return (int)reader.GetByte(posIdIndex);
                        }
                        return 0;
                    }
                }
                catch
                {
                    throw new ApplicationException("Misslyckades med att hämta information från databasen.");
                }
            }
        }

        #endregion

    }
}