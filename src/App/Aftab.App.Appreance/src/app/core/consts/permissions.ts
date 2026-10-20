export const Permissions = {
  Catalog: {
    Create: 'Catalog.Create',
    Read: 'Catalog.Read',
    Update: 'Catalog.Update',
    Delete: 'Catalog.Delete',
  },
  Orders: { View: 'Orders.View', Manage: 'Orders.Manage' },
  Files: { Create: 'Files.Create', ViewWholesalePrice: 'Files.ViewWholesalePrice' },
} as const;
