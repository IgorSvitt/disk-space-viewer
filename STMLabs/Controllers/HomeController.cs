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
        _directoryMethods =  directoryMethods;
    }
    public async Task<IActionResult> Index()
    {
        var directory = await _directoryMethods.GetDirectories("C:\\");

        return View(directory);
    }
    
    public async Task<IActionResult> Directory(string path)
    {
        var directory =  await _directoryMethods.GetDirectories(path);

        return View("Index", directory);
    }
}