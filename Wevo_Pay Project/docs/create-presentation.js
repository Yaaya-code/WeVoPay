const pptxgen = require("pptxgenjs");
const fs = require("fs");
const path = require("path");

const pres = new pptxgen();
pres.layout = "LAYOUT_16x9";
pres.author = "WeVo Pay Team";
pres.title = "WeVo Pay - Project Presentation";
pres.subject = "P2P Wallet to InstaPay Bridge";

const C = {
  navy: "0B1F3A",
  blue: "2563EB",
  blueDark: "1D4ED8",
  ice: "DBEAFE",
  white: "FFFFFF",
  slate: "64748B",
  dark: "0F172A",
  soft: "F8FAFC",
  gold: "F59E0B",
  green: "10B981",
  card: "FFFFFF"
};

const hero = path.join(__dirname, "hero.jpg");
const flow = path.join(__dirname, "flow.jpg");
const dash = path.join(__dirname, "dashboard.jpg");
const sec = path.join(__dirname, "security.jpg");
const logo = path.join(__dirname, "logo.png");

function addFooter(slide, page, total = 12) {
  slide.addText(`WeVo Pay  ·  Project Presentation  ·  ${page}/${total}`, {
    x: 0.5, y: 5.25, w: 9, h: 0.25,
    fontSize: 10, fontFace: "Calibri", color: C.slate, margin: 0
  });
}

// ========== SLIDE 1: Title ==========
{
  const s = pres.addSlide();
  s.addImage({ path: hero, x: 0, y: 0, w: 10, h: 5.625 });
  s.addShape(pres.shapes.RECTANGLE, {
    x: 0, y: 0, w: 10, h: 5.625,
    fill: { color: C.navy, transparency: 45 }
  });
  s.addShape(pres.shapes.RECTANGLE, {
    x: 0, y: 0, w: 0.18, h: 5.625,
    fill: { color: C.blue }
  });
  if (fs.existsSync(logo)) {
    s.addImage({ path: logo, x: 0.55, y: 0.45, w: 0.7, h: 0.7 });
  }
  s.addText("WeVo Pay", {
    x: 1.4, y: 0.55, w: 6, h: 0.5,
    fontSize: 22, fontFace: "Arial", bold: true, color: C.white, margin: 0
  });
  s.addText("Bridge Egyptian Wallets\nto InstaPay", {
    x: 0.55, y: 1.8, w: 8.5, h: 1.6,
    fontSize: 40, fontFace: "Arial", bold: true, color: C.white, margin: 0
  });
  s.addText("A secure P2P escrow-style transfer platform built with ASP.NET Core MVC", {
    x: 0.55, y: 3.55, w: 8, h: 0.5,
    fontSize: 16, fontFace: "Calibri", color: C.ice, margin: 0
  });
  s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
    x: 0.55, y: 4.3, w: 2.4, h: 0.45,
    fill: { color: C.blue }, rectRadius: 0.1
  });
  s.addText("Project Presentation", {
    x: 0.55, y: 4.35, w: 2.4, h: 0.35,
    fontSize: 12, fontFace: "Arial", bold: true, color: C.white, align: "center", margin: 0
  });
}

// ========== SLIDE 2: The Problem ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.soft } });
  s.addText("The Problem", {
    x: 0.5, y: 0.35, w: 9, h: 0.5,
    fontSize: 32, fontFace: "Arial", bold: true, color: C.navy, margin: 0
  });
  s.addText("Why WeVo Pay exists", {
    x: 0.5, y: 0.85, w: 9, h: 0.3,
    fontSize: 14, fontFace: "Calibri", color: C.slate, margin: 0
  });

  const cards = [
    { t: "Wallet locked", d: "Vodafone Cash & local wallets cannot send money straight to InstaPay.", c: "EF4444" },
    { t: "User friction", d: "People need a trusted middle path with clear status tracking.", c: "F59E0B" },
    { t: "No transparency", d: "Without a system, cash-outs are manual, risky, and hard to audit.", c: "8B5CF6" }
  ];
  cards.forEach((card, i) => {
    const x = 0.5 + i * 3.1;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y: 1.5, w: 2.9, h: 3.2,
      fill: { color: C.white },
      shadow: { type: "outer", color: "000000", blur: 12, opacity: 0.08, offset: 3 },
      rectRadius: 0.12
    });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x: x + 0.25, y: 1.8, w: 0.55, h: 0.55,
      fill: { color: card.c }, rectRadius: 0.1
    });
    s.addText(String(i + 1), {
      x: x + 0.25, y: 1.88, w: 0.55, h: 0.4,
      fontSize: 16, bold: true, color: C.white, align: "center", margin: 0
    });
    s.addText(card.t, {
      x: x + 0.25, y: 2.6, w: 2.4, h: 0.5,
      fontSize: 18, bold: true, color: C.dark, margin: 0
    });
    s.addText(card.d, {
      x: x + 0.25, y: 3.2, w: 2.4, h: 1.2,
      fontSize: 13, color: C.slate, margin: 0
    });
  });
  addFooter(s, 2);
}

// ========== SLIDE 3: Solution ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.navy } });
  s.addImage({ path: flow, x: 5.2, y: 0.9, w: 4.4, h: 3.8 });
  s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
    x: 5.2, y: 0.9, w: 4.4, h: 3.8,
    fill: { type: "none" },
    line: { color: "334155", width: 1 },
    rectRadius: 0.1
  });
  s.addText("Our Solution", {
    x: 0.5, y: 0.45, w: 4.5, h: 0.45,
    fontSize: 28, bold: true, color: C.white, margin: 0
  });
  s.addText("Escrow-style bridge", {
    x: 0.5, y: 1.0, w: 4.5, h: 0.35,
    fontSize: 16, color: C.ice, margin: 0
  });
  const steps = [
    "User creates a transfer request",
    "Pays the company wallet (e.g. Vodafone Cash)",
    "Admin verifies the payment",
    "Admin completes payout to InstaPay"
  ];
  steps.forEach((t, i) => {
    const y = 1.6 + i * 0.7;
    s.addShape(pres.shapes.OVAL, {
      x: 0.55, y, w: 0.42, h: 0.42,
      fill: { color: C.blue }
    });
    s.addText(String(i + 1), {
      x: 0.55, y: y + 0.05, w: 0.42, h: 0.32,
      fontSize: 14, bold: true, color: C.white, align: "center", margin: 0
    });
    s.addText(t, {
      x: 1.15, y: y + 0.05, w: 3.8, h: 0.35,
      fontSize: 14, color: C.white, margin: 0
    });
  });
  addFooter(s, 3);
}

// ========== SLIDE 4: Business value ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.soft } });
  s.addText("Business Value", {
    x: 0.5, y: 0.35, w: 9, h: 0.45,
    fontSize: 30, bold: true, color: C.navy, margin: 0
  });
  const vals = [
    { n: "01", t: "Solves a real gap", d: "Wallet → InstaPay that apps cannot do natively" },
    { n: "02", t: "Fee revenue", d: "Configurable % fee on every completed bridge transfer" },
    { n: "03", t: "Trust & control", d: "Admin verification + 1-hour pending timeout" },
    { n: "04", t: "Growth loop", d: "Referral links on registration" }
  ];
  vals.forEach((v, i) => {
    const col = i % 2;
    const row = Math.floor(i / 2);
    const x = 0.5 + col * 4.7;
    const y = 1.15 + row * 1.8;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y, w: 4.4, h: 1.55,
      fill: { color: C.white },
      shadow: { type: "outer", color: "000000", blur: 10, opacity: 0.07, offset: 2 },
      rectRadius: 0.12
    });
    s.addText(v.n, {
      x: x + 0.3, y: y + 0.25, w: 1, h: 0.4,
      fontSize: 22, bold: true, color: C.blue, margin: 0
    });
    s.addText(v.t, {
      x: x + 0.3, y: y + 0.65, w: 3.8, h: 0.35,
      fontSize: 16, bold: true, color: C.dark, margin: 0
    });
    s.addText(v.d, {
      x: x + 0.3, y: y + 1.0, w: 3.8, h: 0.35,
      fontSize: 12, color: C.slate, margin: 0
    });
  });
  addFooter(s, 4);
}

// ========== SLIDE 5: Actors ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.soft } });
  s.addText("Who Uses WeVo Pay?", {
    x: 0.5, y: 0.35, w: 9, h: 0.45,
    fontSize: 30, bold: true, color: C.navy, margin: 0
  });
  const actors = [
    { title: "User", items: ["Create transfers", "Track status + timer", "Messages & rates", "Profile & settings"], color: C.blue },
    { title: "Admin", items: ["Verify payments", "Complete / reject", "Manage wallets", "Set fees & reply chat"], color: "7C3AED" },
    { title: "System", items: ["Auto-cancel 1h", "Hash passwords", "Seed admin", "Cache live rates"], color: C.green }
  ];
  actors.forEach((a, i) => {
    const x = 0.45 + i * 3.15;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y: 1.15, w: 3.0, h: 3.7,
      fill: { color: C.white },
      shadow: { type: "outer", color: "000000", blur: 10, opacity: 0.08, offset: 2 },
      rectRadius: 0.14
    });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y: 1.15, w: 3.0, h: 0.7,
      fill: { color: a.color }, rectRadius: 0.14
    });
    // cover bottom radius of header
    s.addShape(pres.shapes.RECTANGLE, {
      x, y: 1.55, w: 3.0, h: 0.3,
      fill: { color: a.color }
    });
    s.addText(a.title, {
      x, y: 1.3, w: 3.0, h: 0.4,
      fontSize: 18, bold: true, color: C.white, align: "center", margin: 0
    });
    a.items.forEach((item, j) => {
      s.addText(item, {
        x: x + 0.3, y: 2.15 + j * 0.55, w: 2.4, h: 0.4,
        fontSize: 13, color: C.dark, margin: 0
      });
    });
  });
  addFooter(s, 5);
}

// ========== SLIDE 6: Transfer flow visual ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.navy } });
  s.addText("Transfer Lifecycle", {
    x: 0.5, y: 0.3, w: 9, h: 0.4,
    fontSize: 28, bold: true, color: C.white, margin: 0
  });
  const statuses = [
    { t: "Pending", d: "Created\n1h timer", c: "F59E0B" },
    { t: "Verified", d: "Payment\nconfirmed", c: "3B82F6" },
    { t: "Completed", d: "InstaPay\npaid out", c: "10B981" },
    { t: "Rejected", d: "Admin\nrefused", c: "EF4444" },
    { t: "Cancelled", d: "Timeout\nauto", c: "94A3B8" }
  ];
  statuses.forEach((st, i) => {
    const x = 0.4 + i * 1.9;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y: 1.3, w: 1.75, h: 2.4,
      fill: { color: "132#if".replace("if", "33A") },
      rectRadius: 0.12
    });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y: 1.3, w: 1.75, h: 2.4,
      fill: { color: "1E3A5F" },
      rectRadius: 0.12
    });
    s.addShape(pres.shapes.OVAL, {
      x: x + 0.55, y: 1.55, w: 0.65, h: 0.65,
      fill: { color: st.c }
    });
    s.addText(st.t, {
      x, y: 2.4, w: 1.75, h: 0.4,
      fontSize: 13, bold: true, color: C.white, align: "center", margin: 0
    });
    s.addText(st.d, {
      x: x + 0.1, y: 2.9, w: 1.55, h: 0.6,
      fontSize: 11, color: C.ice, align: "center", margin: 0
    });
    if (i < statuses.length - 1) {
      s.addShape(pres.shapes.RIGHT_ARROW, {
        x: x + 1.7, y: 2.3, w: 0.25, h: 0.2,
        fill: { color: C.blue }
      });
    }
  });
  s.addText("Fee = Amount x Fee%   |   Total to send = Amount + Fee   |   Fee% from System Settings", {
    x: 0.5, y: 4.2, w: 9, h: 0.4,
    fontSize: 13, color: C.ice, margin: 0
  });
  s.addText("Pending without admin verification for 1 hour becomes Cancelled automatically.", {
    x: 0.5, y: 4.65, w: 9, h: 0.35,
    fontSize: 13, color: C.gold, margin: 0
  });
  addFooter(s, 6);
}

// ========== SLIDE 7: Tech stack ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.soft } });
  s.addText("Technical Stack", {
    x: 0.5, y: 0.3, w: 9, h: 0.45,
    fontSize: 30, bold: true, color: C.navy, margin: 0
  });
  s.addImage({ path: dash, x: 5.6, y: 1.1, w: 3.9, h: 3.6 });
  const stack = [
    ["Backend", "ASP.NET Core MVC (.NET 10)"],
    ["ORM / DB", "EF Core + SQL Server"],
    ["Auth", "Cookie auth + Roles"],
    ["UI", "Razor + Bootstrap + custom CSS"],
    ["Jobs", "IHostedService auto-cancel"],
    ["APIs", "Live FX + Gold (cached)"]
  ];
  stack.forEach((row, i) => {
    const y = 1.05 + i * 0.6;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x: 0.5, y, w: 4.8, h: 0.5,
      fill: { color: C.white }, rectRadius: 0.08
    });
    s.addText(row[0], {
      x: 0.7, y: y + 0.1, w: 1.5, h: 0.3,
      fontSize: 12, bold: true, color: C.blue, margin: 0
    });
    s.addText(row[1], {
      x: 2.2, y: y + 0.1, w: 2.9, h: 0.3,
      fontSize: 12, color: C.dark, margin: 0
    });
  });
  addFooter(s, 7);
}

// ========== SLIDE 8: Architecture ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.soft } });
  s.addText("Architecture Layers", {
    x: 0.5, y: 0.3, w: 9, h: 0.4,
    fontSize: 28, bold: true, color: C.navy, margin: 0
  });
  const layers = [
    { t: "Views (Razor)", d: "Login, Dashboard, Transfers, Admin, Messages, Rates", c: "3B82F6" },
    { t: "Controllers", d: "HTTP, Authorize, ModelState, redirect / View()", c: "2563EB" },
    { t: "Services", d: "Business rules: fee, verify, complete, expire, chat", c: "1D4ED8" },
    { t: "Data (EF + SQL)", d: "Users, Wallets, TransferRequests, Transactions, Messages", c: "1E3A8A" }
  ];
  layers.forEach((L, i) => {
    const y = 1.0 + i * 1.0;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x: 1.2, y, w: 7.6, h: 0.85,
      fill: { color: L.c }, rectRadius: 0.1
    });
    s.addText(L.t, {
      x: 1.5, y: y + 0.12, w: 7, h: 0.3,
      fontSize: 16, bold: true, color: C.white, margin: 0
    });
    s.addText(L.d, {
      x: 1.5, y: y + 0.42, w: 7, h: 0.3,
      fontSize: 12, color: C.ice, margin: 0
    });
  });
  addFooter(s, 8);
}

// ========== SLIDE 9: Features grid ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.soft } });
  s.addText("Feature Highlights", {
    x: 0.5, y: 0.3, w: 9, h: 0.4,
    fontSize: 28, bold: true, color: C.navy, margin: 0
  });
  const feats = [
    { t: "Transfers", d: "Create, track, details timeline" },
    { t: "1h Timer", d: "Auto-cancel pending requests" },
    { t: "Admin Ops", d: "Verify / Complete / Reject" },
    { t: "Messages", d: "User ↔ Admin support chat" },
    { t: "Rates", d: "Arab FX + gold in EGP" },
    { t: "Referrals", d: "Grow with invite links" }
  ];
  feats.forEach((f, i) => {
    const col = i % 3;
    const row = Math.floor(i / 3);
    const x = 0.5 + col * 3.1;
    const y = 1.1 + row * 1.9;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y, w: 2.9, h: 1.65,
      fill: { color: C.white },
      shadow: { type: "outer", color: "000000", blur: 10, opacity: 0.07, offset: 2 },
      rectRadius: 0.12
    });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x: x + 0.25, y: y + 0.3, w: 0.5, h: 0.5,
      fill: { color: C.ice }, rectRadius: 0.1
    });
    s.addText(f.t, {
      x: x + 0.25, y: y + 0.95, w: 2.4, h: 0.3,
      fontSize: 15, bold: true, color: C.dark, margin: 0
    });
    s.addText(f.d, {
      x: x + 0.25, y: y + 1.25, w: 2.4, h: 0.3,
      fontSize: 12, color: C.slate, margin: 0
    });
  });
  addFooter(s, 9);
}

// ========== SLIDE 10: Security ==========
{
  const s = pres.addSlide();
  s.addImage({ path: sec, x: 0, y: 0, w: 10, h: 5.625 });
  s.addShape(pres.shapes.RECTANGLE, {
    x: 0, y: 0, w: 10, h: 5.625,
    fill: { color: C.navy, transparency: 55 }
  });
  s.addText("Security & Trust", {
    x: 0.5, y: 0.4, w: 9, h: 0.5,
    fontSize: 30, bold: true, color: C.white, margin: 0
  });
  const secs = [
    "Passwords hashed (never plain text)",
    "Cookie authentication + role checks",
    "Anti-forgery tokens on POST forms",
    "Users only access their own transfers",
    "Admin actions are role-gated",
    "Pending requests expire in 1 hour"
  ];
  secs.forEach((t, i) => {
    const col = i % 2;
    const row = Math.floor(i / 2);
    const x = 0.5 + col * 4.7;
    const y = 1.3 + row * 1.1;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x, y, w: 4.4, h: 0.9,
      fill: { color: C.white, transparency: 12 },
      rectRadius: 0.1
    });
    s.addText(t, {
      x: x + 0.3, y: y + 0.28, w: 3.9, h: 0.4,
      fontSize: 14, color: C.white, margin: 0
    });
  });
  addFooter(s, 10);
}

// ========== SLIDE 11: Demo path ==========
{
  const s = pres.addSlide();
  s.addShape(pres.shapes.RECTANGLE, { x: 0, y: 0, w: 10, h: 5.625, fill: { color: C.soft } });
  s.addText("Live Demo Path", {
    x: 0.5, y: 0.3, w: 9, h: 0.45,
    fontSize: 28, bold: true, color: C.navy, margin: 0
  });
  const demo = [
    { n: "1", t: "Admin login", d: "admin / Admin@123 → Dashboard" },
    { n: "2", t: "User transfer", d: "Create request → see fee + 1h timer" },
    { n: "3", t: "Verify & complete", d: "Admin confirms payment → InstaPay done" },
    { n: "4", t: "Extras", d: "Messages · Rates · Referral · Settings" }
  ];
  demo.forEach((d, i) => {
    const y = 1.0 + i * 1.0;
    s.addShape(pres.shapes.OVAL, {
      x: 0.6, y: y + 0.1, w: 0.6, h: 0.6,
      fill: { color: C.blue }
    });
    s.addText(d.n, {
      x: 0.6, y: y + 0.2, w: 0.6, h: 0.4,
      fontSize: 18, bold: true, color: C.white, align: "center", margin: 0
    });
    if (i < demo.length - 1) {
      s.addShape(pres.shapes.RECTANGLE, {
        x: 0.86, y: y + 0.75, w: 0.08, h: 0.35,
        fill: { color: C.ice }
      });
    }
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
      x: 1.5, y, w: 7.8, h: 0.85,
      fill: { color: C.white }, rectRadius: 0.1
    });
    s.addText(d.t, {
      x: 1.8, y: y + 0.12, w: 7, h: 0.3,
      fontSize: 16, bold: true, color: C.dark, margin: 0
    });
    s.addText(d.d, {
      x: 1.8, y: y + 0.45, w: 7, h: 0.3,
      fontSize: 13, color: C.slate, margin: 0
    });
  });
  addFooter(s, 11);
}

// ========== SLIDE 12: Thank you ==========
{
  const s = pres.addSlide();
  s.addImage({ path: hero, x: 0, y: 0, w: 10, h: 5.625 });
  s.addShape(pres.shapes.RECTANGLE, {
    x: 0, y: 0, w: 10, h: 5.625,
    fill: { color: C.navy, transparency: 40 }
  });
  s.addText("Thank You", {
    x: 0.5, y: 1.6, w: 9, h: 0.7,
    fontSize: 44, bold: true, color: C.white, align: "center", margin: 0
  });
  s.addText("Questions welcome — technical details are in the companion guide.", {
    x: 1, y: 2.5, w: 8, h: 0.5,
    fontSize: 16, color: C.ice, align: "center", margin: 0
  });
  s.addShape(pres.shapes.ROUNDED_RECTANGLE, {
    x: 3.0, y: 3.4, w: 4, h: 0.7,
    fill: { color: C.blue }, rectRadius: 0.12
  });
  s.addText("WeVo Pay · Wallet to InstaPay", {
    x: 3.0, y: 3.55, w: 4, h: 0.4,
    fontSize: 13, bold: true, color: C.white, align: "center", margin: 0
  });
  s.addText("Companion: WeVo_Pay_Technical_Guide.pdf", {
    x: 0.5, y: 4.5, w: 9, h: 0.3,
    fontSize: 12, color: C.ice, align: "center", margin: 0
  });
}

const out = path.join(__dirname, "WeVo_Pay_Project_Presentation.pptx");
pres.writeFile({ fileName: out }).then(() => console.log("Wrote", out));
