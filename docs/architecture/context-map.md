# Context Map & Anti-Corruption Layer Specification
**Project:** Printer Repair System (`Identification`, `Diagnostics`, `BAH`)

---

## Stage 1: Subdomain Classification & Bounded Context Canvas

### 1. Subdomain Classification

| Context | Classification | Main Aggregates (Lab 2) | Business Justification | Module Microarchitecture |
| :--- | :--- | :--- | :--- | :--- |
| **Diagnostics** *(Diagnostics & Repair)* | **Core** | `RepairTicket`, `SparePartsInventory` | Fast, accurate printer diagnostics, strict calibration quality control, and reliable parts reservation are our primary market differentiators and the source of most business rule changes. | **Rich Domain Model + Clean Architecture** – Complex state machine transitions and strict invariants require isolated domain logic and fast database-free unit tests. |
| **Identification** | **Supporting** | `ClientAccount`, `RegisteredPrinter` | Registering clients (with NIP/TaxId validation) and tracking supported printer models is necessary for intake, but it does not differentiate us from competing repair shops. | **Transaction Script / Active Record** – Straightforward intake and lookup operations with few state transitions do not justify full tactical DDD overhead. |
| **BAH** *(Billing & Handover)* | **Supporting** | `ServiceInvoice` | Issuing invoices, enforcing payment before hardware release, and calculating storage fees protect revenue, though standard billing workflows rarely change. | **Transaction Script + Thin Domain Guards** – Simple lifecycle rules (`IsPaid`, 14-day grace period) can be enforced cleanly without a heavy event-sourced architecture. |

### 2. Bounded Context Canvas: `Diagnostics` (Core)

| Section | Details |
| :--- | :--- |
| **1. Name & Purpose** | **Diagnostics:** Manages the end-to-end technical repair lifecycle—from ticket submission and cost estimation to spare part reservation, physical repair, calibration testing, and handover to billing. |
| **2. Strategic Classification** | **Core Subdomain** – High business differentiation, frequent operational rule refinements, built in-house with full DDD and Clean Architecture. |
| **3. Ubiquitous Language** | `RepairTicket` (central work order), `EstimateApproved` (client sign-off to begin work), `CalibrationTest` (mandatory quality gate before completion), `SparePartsInventory` (warehouse stock of components), `PartQuantity` (immutable non-negative stock unit), `HandoverToBilling` (final transition releasing the ticket to BAH). |
| **4. Business Rules (Invariants)** | • A repair cannot be marked `Completed` if the final calibration test fails (`CalibrationFailedException`).<br>• Spare parts cannot be reserved if requested quantity exceeds available unreserved stock (`InsufficientStockException`).<br>• `CompleteRepair()` is forbidden while in `Submitted` state; `ApproveEstimate()` is forbidden once `Completed`. |
| **5. Incoming Communication** | **Commands:** `CreateRepairTicketCommand`, `ApproveEstimateCommand`, `CompleteRepairCommand`, `ReserveSparePartCommand`, `DeleteRepairTicketCommand`.<br>**Queries:** `GetRepairTicketByIdQuery`, `GetAllRepairTicketsQuery`. |
| **6. Outgoing Communication** | **Events (Published Language):** `RepairCompleted` (consumed by `BAH` to issue a `ServiceInvoice`).<br>**Outbound Port:** `IPartsSupplierPort` (orders out-of-stock spare parts from the external supplier). |
| **7. Dependencies** | • **Identification `[U]`:** Customer–Supplier (references `ClientAccountId` and `RegisteredPrinterId`).<br>• **Legacy Parts Supplier API `[external U]`:** Anti-Corruption Layer (`IPartsSupplierPort`). |

---

## Stage 2: Context Map

```mermaid
flowchart LR
    subgraph External["External Systems"]
        SUP["Legacy Parts Supplier API<br/>[external U]"]
        PAY["Payment Gateway (Stripe)<br/>[external U]"]
    end

    subgraph System["PrinterRepair Modular Monolith"]
        ID["Identification<br/>Supporting [U]"]
        DIAG["Diagnostics<br/>Core [D / U]"]
        BAH["BAH (Billing & Handover)<br/>Supporting [D]"]
    end

    ID -->|"Customer–Supplier<br/>[U] -> [D]"| DIAG
    DIAG -->|"OHS + Published Language<br/>(RepairCompleted) [U] -> [D]"| BAH
    SUP -->|"Anti-Corruption Layer<br/>[U] -> [D]"| DIAG
    PAY -->|"Conformist + Thin Port<br/>[U] -> [D]"| BAH