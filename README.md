# Unity Runtime Content Loading Demo

This Unity project demonstrates runtime content fetching, parsing, and display from external sources. The application dynamically loads and presents various types of assets including:

- ✅ **Text**
- ✅ **Images**
- ✅ **Videos**
- ✅ **3D Models with Animation**

All content is loaded dynamically during runtime using structured raw links, simulating API-like behavior.

---

## 🔧 Features Implemented

- 📦 **Dynamic Asset Loading**  
  Fetch and display assets (text, images, videos, and 3D models) directly from cloud-hosted locations using raw URLs.

- 🔁 **Runtime Parsing**  
  Dynamically parse data and generate content at runtime without hardcoding.

- 🎞️ **Animated 3D Model Handling**  
  3D models are loaded and animated in runtime to demonstrate full interactivity.

- 🌐 **WebGL Support**  
  The project is exported as a WebGL build to demonstrate cross-platform accessibility in browsers.

---

## 🚀 Getting Started

### 💻 Run the Project in Unity

1. **Clone or Download** this repository.
2. Open the project in **Unity Editor (recommended version: [your Unity version here])**.
3. Play the scene to see the runtime content loading system in action.

---

## 🌍 WebGL Build

You can access the **live WebGL demo** here:  
👉 [WebGL Build Link](https://anuj-chouhan.github.io/Unity-Ar-Assets/ProjectBuild/)

---

## 📁 Repository Structure

### `HostedStuffs/`
This folder contains all the external files that are fetched and loaded into the project at runtime:

- **Images**
- **Videos**
- **3D Models**
- **Text Files**

These assets are accessed using raw GitHub links structured to mimic API endpoints.

> 📝 *Note:* This folder also previously contained a JSON file used for demonstrating JSON parsing and content loading. That feature has been deprecated in favor of direct URL-based asset loading in recent commits.

### `ProjectBuild/`
This folder contains the **WebGL build output**. You can host these files on a server to access the project in a browser.

---

## 📌 Notes

- This project was completed as part of a technical task to demonstrate my ability to work with **runtime asset loading**, **remote content handling**, and **Unity**
