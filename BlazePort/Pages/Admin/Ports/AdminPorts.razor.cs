﻿using BlazePort.Components;
using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Pages.Admin.Ports
{
    public class AdminPortsBase : ComponentBase
    {
        [Inject] BlazePortContext Db { get; set; }
        protected FlyoutPanel EditorPanel { get; set; }
        protected Notification SuccessNotification { get; set; }
        protected PortDetailsGridView[] portsGridView;

        protected PortDetailsForm portForm = new PortDetailsForm();

        protected LocationDetails[] locations;

        protected bool SuccessMessageVisible => LastSuccessfulSave != null;

        protected PortDetails LastSuccessfulSave { get; set; }

        protected override async Task OnInitAsync()
        {
            portsGridView = await LoadPortsViewModel();
            locations = await Db.Locations.ToArrayAsync();
        }

        protected async Task CloseEditor()
        {
            await EditorPanel.HideAsync();
        }

        protected async Task OpenEditor()
        {
            await EditorPanel.ShowAsync();
        }

        protected async Task<PortDetailsGridView[]> LoadPortsViewModel() =>
                await Db.PortDetails.Include(p => p.Location)
                    .Select(p => PortDetailsGridView.FromPort(p))
                    .ToArrayAsync();

        protected async Task SaveLocation()
        {
            var item = portForm.ToPortDetails();
            Db.PortDetails.Add(item);
                                  
            await Db.SaveChangesAsync();

            LastSuccessfulSave = item;

            portsGridView = await LoadPortsViewModel();

            portForm = new PortDetailsForm();

            await EditorPanel.HideAsync();

            await SuccessNotification.Show();
        }
    }
}