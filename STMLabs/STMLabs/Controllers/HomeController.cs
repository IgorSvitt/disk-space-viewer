using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using STMLabs.Models;
using STMLabs.Service;
using Directory = System.IO.Directory;


namespace STMLabs.Controllers;

public class HomeController : Controller
{
    private readonly DirectoryMethods _directoryMethods;

    public HomeController(DirectoryMethods directoryMethods)
    {
        _directoryMethods = directoryMethods;
    }
    public IActionResult Index()
    {
        var directory = _directoryMethods.GetDirectories("/app");

        return View(directory);
    }
    
    public IActionResult Directory(string path)
    {
        var directory = _directoryMethods.GetDirectories(path);

        return View("Index", directory);
    }
}