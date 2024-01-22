
using BEPZA_MEDICAL.DAL.PGM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace BEPZA_MEDICAL.Domain.PGM
{
    public class CustomPropertiesService
    {
        ERP_BEPZAPGMEntities _pgmContext;

        public CustomPropertiesService()
        {
            _pgmContext = new ERP_BEPZAPGMEntities();
        }

        public string RetriveDisplayName(string modelName, string propertyName, String defaultDisplayText = "")
        {
            var customPropertyAttribute = _pgmContext.CustomPropertyAttributes.FirstOrDefault(c => c.ModelName.Contains(modelName) && c.PropertyName == propertyName);

            var displayText = String.Empty;

            if (customPropertyAttribute != null)
                displayText = customPropertyAttribute.DisplayText;

            if (String.IsNullOrEmpty(displayText))
            {
                if (String.IsNullOrEmpty(defaultDisplayText))
                    displayText = propertyName.SplitWords();
                else
                    displayText = defaultDisplayText;
            }

            return displayText;
        }
    }

    #region Custom Display
    public class CustomDisplayAttribute : Attribute
    {
        public CustomDisplayAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    public class ConventionalModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        /*
            public class MyViewModel
            {
              public MyPropertyType PropertyName { get; set; }
            }

            containerType = MyViewModel
            modelType = MyPropertyType
            propertyName = PropertyName
        */
        protected override ModelMetadata CreateMetadata(
            IEnumerable<Attribute> attributes
            , Type containerType
            , Func<object> modelAccessor
            , Type modelType
            , string propertyName)
        {
            ModelMetadata modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType,
                propertyName);

            var displayAttribute = containerType == null
                ? null as CustomDisplayAttribute
                : containerType.GetProperty(propertyName)
                    .GetCustomAttributes(false)
                    .OfType<CustomDisplayAttribute>()
                    .FirstOrDefault();

            // if CustomDisplay attribute applied
            if (displayAttribute != null)
            {
                var _customPropertiesService = new CustomPropertiesService();
                var displayName = _customPropertiesService.RetriveDisplayName(containerType.ToString(), propertyName, displayAttribute.Name);

                modelMetadata.DisplayName = displayName;
            }
            else if (modelMetadata.DisplayName == null)
            {
                modelMetadata.DisplayName = modelMetadata.PropertyName.SplitWords();
            }

            return modelMetadata;
        }
    }

    public static class StringExtensions
    {
        public static string SplitWords(this string value)
        {
            return value != null ? Regex.Replace(value, "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 ").Trim() : null;
        }
    }
    #endregion

}
