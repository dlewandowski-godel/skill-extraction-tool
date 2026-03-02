import { DRAWER_WIDTH, Sidebar } from "@/components/layout/Sidebar";
import MenuIcon from "@mui/icons-material/Menu";
import { AppBar, Box, IconButton, Toolbar, Typography } from "@mui/material";
import { useState } from "react";
import { Outlet } from "react-router-dom";

export function AppLayout() {
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      {/* Mobile top bar */}
      <AppBar
        position="fixed"
        sx={{
          display: { md: "none" },
          zIndex: (theme) => theme.zIndex.drawer + 1,
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            edge="start"
            onClick={() => setMobileOpen((o) => !o)}
            aria-label="open navigation"
            sx={{ mr: 2 }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap>
            SkillExtractor
          </Typography>
        </Toolbar>
      </AppBar>

      {/* Sidebar */}
      <Sidebar
        mobileOpen={mobileOpen}
        onMobileClose={() => setMobileOpen(false)}
      />

      {/* Main content */}
      <Box
        component="main"
        sx={{
          flex: 1,
          minWidth: 0,
          // Offset for mobile AppBar height
          mt: { xs: 7, md: 0 },
          // Offset for desktop permanent drawer
          ml: { md: 0 },
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          overflow: "auto",
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
}
