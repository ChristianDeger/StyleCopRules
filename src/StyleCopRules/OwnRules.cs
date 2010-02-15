namespace StyleCopRules
{
    using System;
    using Microsoft.StyleCop;
    using Microsoft.StyleCop.CSharp;

    [SourceAnalyzer(typeof(CsParser))]
    public class OwnRules : SourceAnalyzer
    {
        public override void AnalyzeDocument(CodeDocument document)
        {
            Param.RequireNotNull(document, "document");
            var csDocument = (CsDocument)document;
            if ((csDocument.RootElement != null) && !csDocument.RootElement.Generated)
            {
                csDocument.WalkDocument(VisitElement);
            }
        }

        private bool VisitElement(CsElement element, CsElement parentelement, object context)
        {
            if (element.ElementType == ElementType.Field)
            {
                VisitField((Field)element);
            }

            return true;
        }

        private void VisitField(Field field)
        {
            PrivateFieldNameMustStartWithUnderscoreFollowedByLowerCase(field);
        }

        private void PrivateFieldNameMustStartWithUnderscoreFollowedByLowerCase(Field field)
        {
            if (field.Generated)
            {
                return;
            }

            if (field.Const)
            {
                return;
            }

            if (IsReadonlyStatic(field))
            {
                return;
            }

            if (ViolatesFieldNameRule(field.Declaration.Name))
            {
                AddViolation(
                    field,
                    "PrivateFieldNameMustStartWithUnderscoreFollowedByLowerCase",
                    new object[0]);
            }
        }

        private static bool ViolatesFieldNameRule(string fieldName)
        {
            if (!fieldName.StartsWith("_"))
            {
                return true;
            }

            return SecondCharNotLowercase(fieldName);
        }

        private static bool SecondCharNotLowercase(string fieldName)
        {
            if (fieldName.Length < 2)
            {
                return false;
            }

            var secondChar = fieldName.Substring(1, 1);
            return !Char.IsLower(secondChar[0]);
        }

        private static bool IsReadonlyStatic(Field field)
        {
            return field.Readonly && field.Declaration.ContainsModifier(new[] { CsTokenType.Static });
        }
    }
}