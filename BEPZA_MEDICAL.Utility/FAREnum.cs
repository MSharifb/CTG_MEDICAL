using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPZA_MEDICAL.Utility
{
    public static class FAREnum
    {
        public enum FARAssetType { NewAsset = 1, ExistingAsset };
        public enum FARAssetStatus { Good = 1, Repairable, OnSupport, Disposed, Sold }
        public enum FARBeneficiaryType { Employee = 1, Department, Other}
    }
}
