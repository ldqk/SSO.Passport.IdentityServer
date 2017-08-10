using System.ComponentModel.DataAnnotations;

namespace Models.Enum
{
    public enum FunctionType
    {
        [Display(Name = "菜单权限")]
        Menu = 1,

        [Display(Name = "操作权限")]
        Operating = 2
    }
}