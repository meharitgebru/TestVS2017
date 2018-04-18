using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WMS_LNE.Models;

namespace WMS_LNE.TagHelpers
{
      
    [HtmlTargetElement("table", Attributes = "source-model")]
    public class TableTagHelper : TagHelper
    {
        [HtmlAttributeName("source-model")]
        public ModelExpression DataModel { get; set; }

        [HtmlAttributeName("id")]
        public string ID { get; set; }

        [HtmlAttributeName("sort-by")]
        public string SortBy { get; set; }

        [HtmlAttributeName("sort-direction")]
        public string SortDirection { get; set; }

        [HtmlAttributeName("sortable")]
        public bool Sortable { get; set; }

        [HtmlAttributeName("controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("action")]
        public string Action { get; set; }

        [HtmlAttributeName("query-string")]
        public string QueryString { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IEnumerable model = DataModel.Model as IEnumerable;
            
            if (model == null)
            {
                return;
            }
            else
            {
               StringBuilder tableTag = new StringBuilder();
                
                //Getting properties for List Type
                Type listType = Type.GetType(((System.Reflection.TypeInfo)(model.GetType()).GenericTypeArguments[0]).FullName);
                var properties = listType.GetProperties();

                
                tableTag.Append("<thead><tr role='row'>");

                //Simple table, header not sortable
                if(!Sortable)
                {
                    foreach (var property in properties)
                    {
                        tableTag.Append("<th >");

                        //If the property has display name, get the display name
                        var isDisplayNameAttributeDefined = Attribute.IsDefined(property, typeof(DisplayNameAttribute));
                        if (isDisplayNameAttributeDefined)
                        {
                            DisplayNameAttribute dna = (DisplayNameAttribute)Attribute.GetCustomAttribute(property, typeof(DisplayNameAttribute));
                            tableTag.Append(dna.DisplayName);
                        }
                        else
                        {
                            tableTag.Append(property.Name.ToString());
                        }

                        tableTag.Append("</th>");
                    }
                }
                else   // for sortable table
                {
                    string linkPretextTag = "<a href = '/" + Controller + "/" + Action;
                    QueryString = QueryString ?? "";

                    foreach (var property in properties)
                    {

                        string columnName = property.Name.ToString();
                        var isDisplayNameAttributeDefined = Attribute.IsDefined(property, typeof(DisplayNameAttribute));

                        if (isDisplayNameAttributeDefined)
                        {
                            DisplayNameAttribute dna = (DisplayNameAttribute)Attribute.GetCustomAttribute(property, typeof(DisplayNameAttribute));
                            columnName = dna.DisplayName;
                        }

                        tableTag.Append("<th aria-controls='");
                        tableTag.Append(ID);


                        if (property.Name.ToString().ToLower() == SortBy.ToLower())
                        {
                            //if sorted descending 
                            if (SortDirection.ToLower() == "desc")
                            {
                                tableTag.Append("' class='sorting_desc' aria-sort='descending' aria-label='");
                                tableTag.Append(columnName);
                                tableTag.Append(": activate to sort column ascending' >");

                                tableTag.Append(linkPretextTag);
                                tableTag.Append("?sortorder=");
                                tableTag.Append(property.Name.ToString());
                                tableTag.Append(QueryString);
                                tableTag.Append("'>");
                                // sb.Append("&sortDirection=asc'>");
                            }
                            else
                            {
                                tableTag.Append("' class='sorting_asc' aria-sort='ascending' aria-label='");
                                tableTag.Append(columnName);
                                tableTag.Append(": activate to sort column descending' >");

                                tableTag.Append(linkPretextTag);
                                tableTag.Append("?sortorder=");
                                tableTag.Append(property.Name.ToString());
                                tableTag.Append(QueryString);
                                tableTag.Append("&sortDirection=desc'>");
                            }
                        }
                        else
                        {
                            tableTag.Append("' class='sorting' aria-label='");
                            tableTag.Append(columnName);
                            tableTag.Append(": activate to sort column ascending' >");

                            tableTag.Append(linkPretextTag);
                            tableTag.Append("?sortorder=");
                            tableTag.Append(property.Name.ToString());
                            tableTag.Append(QueryString);
                            tableTag.Append("'>");
                            // sb.Append("&sortDirection=asc'>");
                        }

                        tableTag.Append(columnName);
                        tableTag.Append("</a></th>");
                    }
                }


                tableTag.Append("</tr></thead>");
                //Generate data
                foreach (var m in model)
                {
                    PropertyInfo[] dataProperties = m.GetType().GetProperties();
                    tableTag.Append("<tr>");
                    for (int i = 0; i < dataProperties.Length; i++)
                    {
                        tableTag.Append("<td>" + m.GetType().GetProperty(dataProperties[i].Name).GetValue(m, null) + "</td>");
                     }
                    tableTag.Append("</tr>");
                }
               // tableTag.Append("<colgroup> <col span = '2' style = 'background-color:red' > <col style = 'background-color:yellow' ></colgroup>");
                output.Content.SetHtmlContent(tableTag.ToString());
            }
        }
    }

    public static class DataManager
    {
        public static List<Person> GetPersons()
        {
            List<Person> lst = new List<Person>();
            lst.Add(new Person { Id = 1, Name = "Ovais Mehboob", Position = "Solution Architect" ,  PageSizeId = PageSize.SeventyFive });
            lst.Add(new Person { Id = 2, Name = "Khusro Habib", Position = "Development Manager", PageSizeId = PageSize.All });
            lst.Add(new Person { Id = 3, Name = "David Salcedo", Position = "Hardware Consultant", PageSizeId = PageSize.Hundered });
            lst.Add(new Person { Id = 4, Name = "Janet Bauer", Position = "Sales Director", PageSizeId = PageSize.SeventyFive });
            lst.Add(new Person { Id = 5, Name = "Asim Khan", Position = "Software Engineer", PageSizeId = PageSize.Fifty });
            return lst;
        }

    }
    public class Person
    {
        [DisplayName("Person ID")]
        public int Id { set; get; }
        [DisplayName("Full Name")]
        public string Name { set; get; }
        public string Position { set; get; }
        public PageSize PageSizeId { get; set; }
    }
}
