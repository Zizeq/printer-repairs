# Aggregates, Value Objects, and Invariants
**Project:** Printer Repair System (Identification, Diagnostics & Repair, BAH)

## 1. Value Objects replacing primitive types

| Value Object (Name) | Component Attributes & Types | Hard Self-Validation Rules (Creation) | Immutable Operation Example |
| :--- | :--- | :--- | :--- |
| **Money**<br>*(Used in ServiceInvoice for repair cost and payments)* | `decimal Amount`<br>`string Currency` | Amount cannot be negative (zero is allowed for fully discounted invoices). Currency code must be exactly "PLN" (`throw new InvalidCurrencyException()`). | `public Money Add(Money other)`<br>Throws a `CurrencyMismatchException` if currencies differ, otherwise returns a new `Money` instance with the summed amount. |
| **PartQuantity**<br>*(Used in SparePartsInventory for stock levels)* | `int Value` | Initial quantity cannot be negative (`throw new InvalidQuantityException()`). | `public PartQuantity Decrease(int amount)`<br>Rejects amounts less than or equal to zero and amounts greater than the current quantity, throwing an `InvalidQuantityException`. Otherwise, returns a new `PartQuantity` with the reduced value. |
| **TaxId**<br>*(Used in ClientAccount for business NIP)* | `string Value` | Cannot be null or empty. After removing spaces and hyphens, the value must contain exactly 10 digits and pass the Polish NIP checksum validation (`throw new InvalidTaxIdException()`). | `public TaxId Normalize()`<br>Returns a new `TaxId` with all spaces and hyphens stripped out, leaving the original input intact. |
| **PrinterModel**<br>*(Used in RepairTicket and RegisteredPrinter)* | `string Brand`<br>`string ModelName` | Neither attribute can be null, empty, or whitespace-only, and both must not exceed 100 characters in length (`throw new InvalidPrinterModelException()`). | `public PrinterModel WithModelName(string newModelName)`<br>Returns a brand new `PrinterModel` instance with the updated model name, ensuring the original object remains safely immutable. |

## 2. Strongly Typed Identifiers

To prevent "Primitive Obsession" and the risk of accidentally swapping raw GUIDs in method parameters (e.g., passing a ticket ID where a client ID was expected), the following core entities use strongly typed wrappers around their `Guid` values:

* **`ClientAccountId`**: Identifies the `ClientAccount` aggregate in the Identification context.
* **`RegisteredPrinterId`**: Identifies the `RegisteredPrinter` aggregate in the Identification context.
* **`RepairTicketId`**: Identifies the `RepairTicket` aggregate in the Diagnostics & Repair context.
* **`SparePartsInventoryId`**: Identifies the `SparePartsInventory` aggregate in the Diagnostics & Repair context.
* **`ServiceInvoiceId`**: Identifies the `ServiceInvoice` aggregate in the Billing & Handover (BAH) context.

By passing `RepairTicketId` instead of a primitive `Guid`, the C# compiler guarantees at compile-time that IDs cannot be silently swapped.