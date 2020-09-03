using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TH.WebUI.Modules.Door.Controllers
{
    using Base.Controllers;
    using TH.Core.Modules.Door;
    using TH.Core.Modules.Door.Entities;
    public class DoorController : CoreController
    {
        private string views = "~/modules/door/views/";
        DoorManager dMan = new DoorManager();


        public ActionResult Content()
        {
            return View(views + "_Content.cshtml");
        }

        [ActionName("render-create-new-door")]
        public ActionResult RenderCreateNewDoor()
        {
            return View(views + "Components/_CreateNewDoor.cshtml");
        }

        [ActionName("save-create-new-door")]
        [HttpPost]
        public ActionResult SaveCreateNewDoor(Door door)
        {
            dMan.SaveDoor(door);
            return Json(new { message = "Created new door" });
        }

        [ActionName("door-details")]
        public ActionResult DoorDetails(int id)
        {
            Door door = dMan.GetDoor(id, kind: true, connection: true, glass: true);

            // Merge childs into door
            dMan.MergeAllIntoDoors(new List<Door>() { door });


            ViewData["door"] = door;
            return View(views + "_DoorDetails.cshtml");
        }
    }
}