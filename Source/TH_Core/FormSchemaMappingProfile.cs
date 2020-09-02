using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH.Core.Tools.Form.Models;

namespace TH.Core
{
    public class FormSchemaMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "FormSchemaMappingProfile"; }
        }

        protected override void Configure()
        {
            //Mapper.CreateMap<FormLayout>

            Mapper.CreateMap<FormElement, ValidationInfo>()
                .ForMember(x => x.HtmlName, x => x.ResolveUsing(y => y.Name));
        }
    }
}
