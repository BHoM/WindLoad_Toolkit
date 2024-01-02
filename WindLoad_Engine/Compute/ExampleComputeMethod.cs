/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;


namespace BH.Engine.Adapters.WindLoad
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Description of the method. Will appear in the UI tooltip.")]
        [Input("someInput1", "Description of the input. Will appear in the UI tooltip.")]
        [Input("someInput2", "Description of the input. Will appear in the UI tooltip.")]
        [Output("outputName", "Description of the output. Will appear in the UI tooltip.")]
        public static string ExampleComputeMethod(string data = "wind", string EC = "din-en-1991-1-3", string location = "messe,duesseldorf",string email = "john.doe@134.com",string hash = "123")
        {
            HttpClient Dlubal = new HttpClient();

            Dlubal.BaseAddress = new Uri("https://external-crm.dlubal.com/loadzones/data.aspx");


            // This method will appear in every UI (e.g. Grasshopper) as a component.
            // Find it using the CTRL+Shift+B search bar, or by navigating the `Compute` component (Engine tab) right click menu.
            // throw new NotImplementedException();
            //deserialisers - newtonsoft 

            return GetAsync(Dlubal, data, EC, location, email, hash ).Result;
        }

        /***************************************************/

        private static async Task<string> GetAsync(HttpClient httpClient, string data = "wind", string EC = "din-en-1991-1-3", string location = "messe,duesseldorf", string email = "john.doe@134.com", string hash = "123")
        {
            HttpResponseMessage response = await httpClient.GetAsync($"https://external-crm.dlubal.com/loadzones/data.aspx?map={data}-{EC}&place={location}&language=en&login={email}&hash={hash}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }
    }
}


