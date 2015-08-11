using System.Web;
using WebService;

[assembly: PreApplicationStartMethod(typeof(Initializer), "AppInitialize")]